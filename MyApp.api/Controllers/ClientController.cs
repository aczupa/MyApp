
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.api.Data;
using MyApp.api.Models;
using MyApp.api.Models.DTOs;
using System.Security.Claims;

namespace MyApp.api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientController(AppDbContext context)
        {
            _context = context;
        }

        // GET : api/client
        // Liste des clients avec les informations fiscales et la TVA
        [HttpGet]
        public async Task<IActionResult> Get(
            int? annee,
            int? mois,
            int? coworkerId)
        {
            var clients = await _context.Clients
                .Include(c => c.Coworker)
                .Where(c => coworkerId == null || c.CoworkerId == coworkerId)
                .ToListAsync();

            var filtre = annee is not null && mois is >= 1 and <= 12;

            var admin = User.IsInRole("Admin");
            var moi = await CoworkerIdCourantAsync();

            if (filtre)
            {
                clients = clients
                    .Where(c => c.GetJourTva(mois!.Value) != null)
                    .OrderBy(c => c.GetJourTva(mois!.Value))
                    .ToList();
            }

            var validees = filtre
                ? (await _context.ValidationsTva
                    .Where(v => v.Annee == annee && v.Mois == mois)
                    .Select(v => v.ClientId)
                    .ToListAsync()).ToHashSet()
                : new HashSet<int>();

            string Tva(Client c)
            {
                if (c.PeriodiciteTva == PeriodiciteTva.NonConcerne)
                    return c.PeriodiciteTva.GetDisplayName();

                if (!filtre)
                    return $"{c.PeriodiciteTva.GetDisplayName()}, le {c.JourTva}";

                var jour = Math.Min(
                    c.JourTva!.Value,
                    DateTime.DaysInMonth(annee!.Value, mois!.Value)
                );

                return $"{jour:00}/{mois:00}/{annee}";
            }

            return Ok(clients.Select(c => new
            {
                c.Id,
                c.DenominationSociale,

                FormeJuridique = c.FormeJuridique.GetDisplayName(),

                c.Adresse,
                c.Ville,
                c.CodePostal,
                c.Pays,
                c.Telephone,
                c.Email,
                c.RepresentantLegal,

                // Informations fiscales
                ModeImposition = c.ModeImposition.ToString(),
                ModeImpositionLabel = c.ModeImposition.GetDisplayName(),

                RegimeFiscal = c.RegimeFiscal.ToString(),
                RegimeFiscalLabel = c.RegimeFiscal.GetDisplayName(),

                c.CoworkerId,
                Coworker = c.Coworker?.Nom,

                Tva = Tva(c),
                Validee = validees.Contains(c.Id),

                PeutValider = admin ||
                    (c.CoworkerId != null && c.CoworkerId == moi)
            }));
        }

        // POST : api/client
        // Création d'un client
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateClientDto dto)
        {
            var client = new Client
            {
                DenominationSociale = dto.DenominationSociale,
                FormeJuridique = dto.FormeJuridique,
                Adresse = dto.Adresse,
                Ville = dto.Ville,
                CodePostal = dto.CodePostal,
                Pays = dto.Pays,
                Telephone = dto.Telephone,
                Email = dto.Email,
                RepresentantLegal = dto.RepresentantLegal,

                PeriodiciteTva = dto.PeriodiciteTva,
                JourTva = dto.JourTva,

                CoworkerId = dto.CoworkerId,

                ModeImposition = dto.ModeImposition,
                RegimeFiscal = dto.RegimeFiscal
            };

            _context.Clients.Add(client);

            await _context.SaveChangesAsync();

            return Ok(new { client.Id });
        }

        // GET : api/client/formes-juridiques
        // Liste des formes juridiques
        [HttpGet("formes-juridiques")]
        public IActionResult GetFormesJuridiques()
        {
            return Ok(
                Enum.GetValues<FormeJuridique>()
                    .Select(f => new
                    {
                        value = f.ToString(),
                        label = f.GetDisplayName(),
                        monaco = f.EstMonegasque()
                    })
            );
        }

        // GET : api/client/regimes-fiscaux
        // Liste des modes d'imposition et des régimes fiscaux
        [HttpGet("regimes-fiscaux")]
        public IActionResult GetRegimesFiscaux()
        {
            return Ok(new
            {
                modes = Enum.GetValues<ModeImposition>()
                    .Select(m => new
                    {
                        value = m.ToString(),
                        label = m.GetDisplayName(),

                        systeme = m switch
                        {
                            ModeImposition.IR or ModeImposition.IS => "FR",
                            ModeImposition.ISB or ModeImposition.NonSoumisIsb => "MC",
                            _ => null
                        }
                    }),

                regimes = Enum.GetValues<RegimeFiscal>()
                    .Select(r => new
                    {
                        value = r.ToString(),
                        label = r.GetDisplayName()
                    })
            });
        }

        // GET : api/client/periodicites-tva
        // Liste des périodicités de TVA
        [HttpGet("periodicites-tva")]
        public IActionResult GetPeriodicitesTva()
        {
            return Ok(
                Enum.GetValues<PeriodiciteTva>()
                    .Select(p => new
                    {
                        value = p.ToString(),
                        label = p.GetDisplayName()
                    })
            );
        }

        // GET : api/client/{id}
        // Détails d'un client
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetClientById(int id, int? annee)
        {
            var c = await _context.Clients
                .Include(x => x.Coworker)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (c == null)
                return NotFound();

            var an = annee is >= 2000 and <= 2100
                ? annee.Value
                : DateTime.Today.Year;

            var peutValider = await PeutValiderAsync(c);

            var validees = (await _context.ValidationsTva
                .Where(v => v.ClientId == id && v.Annee == an)
                .Select(v => v.Mois)
                .ToListAsync())
                .ToHashSet();

            return Ok(new
            {
                c.Id,
                c.DenominationSociale,

                FormeJuridique = c.FormeJuridique.ToString(),
                FormeJuridiqueLabel = c.FormeJuridique.GetDisplayName(),

                c.Adresse,
                c.Ville,
                c.CodePostal,
                c.Pays,
                c.Telephone,
                c.Email,
                c.RepresentantLegal,

                // Informations fiscales
                ModeImposition = c.ModeImposition.ToString(),
                ModeImpositionLabel = c.ModeImposition.GetDisplayName(),

                RegimeFiscal = c.RegimeFiscal.ToString(),
                RegimeFiscalLabel = c.RegimeFiscal.GetDisplayName(),

                PeriodiciteTva = c.PeriodiciteTva.ToString(),
                PeriodiciteTvaLabel = c.PeriodiciteTva.GetDisplayName(),

                c.JourTva,
                c.CoworkerId,

                CoworkerNom = c.Coworker?.Nom,

                PeutValider = peutValider,
                Annee = an,

                Echeances = Enumerable.Range(1, 12).Select(m =>
                {
                    var jour = c.GetJourTva(m);

                    return new
                    {
                        mois = m,
                        jour,
                        validee = jour != null && validees.Contains(m)
                    };
                })
            });
        }

        // PUT : api/client/{id}
        // Modification d'un client
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateClient(int id, CreateClientDto dto)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
                return NotFound();

            client.DenominationSociale = dto.DenominationSociale;
            client.FormeJuridique = dto.FormeJuridique;
            client.Adresse = dto.Adresse;
            client.Ville = dto.Ville;
            client.CodePostal = dto.CodePostal;
            client.Pays = dto.Pays;
            client.Telephone = dto.Telephone;
            client.Email = dto.Email;
            client.RepresentantLegal = dto.RepresentantLegal;

            client.PeriodiciteTva = dto.PeriodiciteTva;
            client.JourTva = dto.JourTva;

            client.CoworkerId = dto.CoworkerId;

            client.ModeImposition = dto.ModeImposition;
            client.RegimeFiscal = dto.RegimeFiscal;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE : api/client/{id}
        // Suppression d'un client
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
                return NotFound();

            _context.Clients.Remove(client);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT : api/client/{id}/tva-validation
        // Validation ou annulation de la validation d'une TVA
        [HttpPut("{id:int}/tva-validation")]
        public async Task<IActionResult> SetTvaValidation(
            int id,
            ValidationTvaDto dto)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
                return NotFound();

            if (!await PeutValiderAsync(client))
                return StatusCode(StatusCodes.Status403Forbidden);

            var existante = await _context.ValidationsTva
                .FirstOrDefaultAsync(v =>
                    v.ClientId == id &&
                    v.Annee == dto.Annee &&
                    v.Mois == dto.Mois
                );

            if (dto.Validee && existante == null)
            {
                _context.ValidationsTva.Add(new ValidationTva
                {
                    ClientId = id,
                    Annee = dto.Annee,
                    Mois = dto.Mois,
                    DateValidation = DateTime.UtcNow
                });
            }
            else if (!dto.Validee && existante != null)
            {
                _context.ValidationsTva.Remove(existante);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Récupération de l'identifiant du coworker connecté
        private async Task<int?> CoworkerIdCourantAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return await _context.Coworkers
                .Where(w => w.UserId == userId)
                .Select(w => (int?)w.Id)
                .FirstOrDefaultAsync();
        }

        // Vérification des droits de validation de TVA
        private async Task<bool> PeutValiderAsync(Client c)
        {
            if (User.IsInRole("Admin"))
                return true;

            return c.CoworkerId != null &&
                   c.CoworkerId == await CoworkerIdCourantAsync();
        }
    }
}
