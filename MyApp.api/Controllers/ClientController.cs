using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.api.Data;
using MyApp.api.Models;
using MyApp.api.Models.DTOs;

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
        [HttpGet]
        public async Task<IActionResult> Get(int? annee, int? mois)
        {
            var clients = await _context.Clients.ToListAsync();
            var filtre = annee is not null && mois is >= 1 and <= 12;

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

                var jour = Math.Min(c.JourTva!.Value, DateTime.DaysInMonth(annee!.Value, mois!.Value));
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
                Tva = Tva(c)
            }));
        }

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
                JourTva = dto.JourTva
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return Ok(new { client.Id });
        }

        [HttpGet("formes-juridiques")]
        public IActionResult GetFormesJuridiques()
        {
            return Ok(Enum.GetValues<FormeJuridique>()
                .Select(f => new { value = f.ToString(), label = f.GetDisplayName() }));
        }

        [HttpGet("periodicites-tva")]
        public IActionResult GetPeriodicitesTva()
        {
            return Ok(Enum.GetValues<PeriodiciteTva>()
                .Select(p => new { value = p.ToString(), label = p.GetDisplayName() }));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetClientById(int id)
        {
            var c = await _context.Clients.FindAsync(id);
            if (c == null) return NotFound();

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
                PeriodiciteTva = c.PeriodiciteTva.ToString(),
                PeriodiciteTvaLabel = c.PeriodiciteTva.GetDisplayName(),
                c.JourTva,
                Echeances = Enumerable.Range(1, 12).Select(m => new { mois = m, jour = c.GetJourTva(m) })
            });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateClient(int id, CreateClientDto dto)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

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

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}/tva-validation")]
        
        public async Task<IActionResult> SetTvaValidation(int id, ValidationTvaDto dto)
        {
            if (!await _context.Clients.AnyAsync(c => c.Id == id)) return NotFound();

            var existante = await _context.ValidationsTva
                .FirstOrDefaultAsync(v => v.ClientId == id && v.Annee == dto.Annee && v.Mois == dto.Mois);

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
    }
}