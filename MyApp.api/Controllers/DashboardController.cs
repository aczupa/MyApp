using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.api.Data;
using MyApp.api.Models;
using System.Globalization;
using System.Security.Claims;

namespace MyApp.api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var aujourdhui = DateOnly.FromDateTime(DateTime.Today);
            var dans7Jours = aujourdhui.AddDays(7);

            var clients = await _context.Clients.Include(c => c.Coworker).ToListAsync();

            var periodes = new[] { aujourdhui, dans7Jours }
                .Select(d => (d.Year, d.Month))
                .Distinct()
                .ToList();
            var annees = periodes.Select(p => p.Year).Distinct().ToList();

            var validations = (await _context.ValidationsTva
                    .Where(v => annees.Contains(v.Annee))
                    .Select(v => new { v.ClientId, v.Annee, v.Mois })
                    .ToListAsync())
                .Select(v => (v.ClientId, v.Annee, v.Mois))
                .ToHashSet();

            // Échéances du mois en cours, avec leur statut
            var ceMois = clients
                .Select(c => new { Client = c, Date = Echeance(c, aujourdhui.Year, aujourdhui.Month) })
                .Where(x => x.Date != null)
                .Select(x =>
                {
                    var validee = validations.Contains((x.Client.Id, aujourdhui.Year, aujourdhui.Month));
                    var statut = validee ? "validee" : x.Date!.Value < aujourdhui ? "retard" : "avalider";
                    return new { x.Client, Statut = statut };
                })
                .ToList();

            // Portefeuille de l'utilisateur connecté (s'il est coworker)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var monCoworker = await _context.Coworkers.FirstOrDefaultAsync(w => w.UserId == userId);

            object? moi = monCoworker == null
                ? null
                : new
                {
                    nom = monCoworker.Nom,
                    stats = Stats(ceMois.Where(x => x.Client.CoworkerId == monCoworker.Id).Select(x => x.Statut))
                };

            // Échéances des 7 prochains jours (peut déborder sur le mois suivant)
            var prochaines = periodes
                .SelectMany(p => clients.Select(c => new { Client = c, Date = Echeance(c, p.Year, p.Month) }))
                .Where(x => x.Date != null && x.Date >= aujourdhui && x.Date <= dans7Jours)
                .OrderBy(x => x.Date)
                .ThenBy(x => x.Client.DenominationSociale)
                .Select(x => new
                {
                    x.Client.Id,
                    x.Client.DenominationSociale,
                    Coworker = x.Client.Coworker?.Nom,
                    Date = x.Date!.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Jours = x.Date!.Value.DayNumber - aujourdhui.DayNumber,
                    Validee = validations.Contains((x.Client.Id, x.Date!.Value.Year, x.Date!.Value.Month))
                })
                .ToList();

            // Charge de chaque coworker pour le mois en cours
            var coworkers = await _context.Coworkers.OrderBy(w => w.Nom).ToListAsync();

            var charge = coworkers.Select(w => new
            {
                w.Id,
                w.Nom,
                NbClients = clients.Count(c => c.CoworkerId == w.Id),
                Stats = Stats(ceMois.Where(x => x.Client.CoworkerId == w.Id).Select(x => x.Statut))
            });

            return Ok(new
            {
                annee = aujourdhui.Year,
                mois = aujourdhui.Month,
                totaux = Stats(ceMois.Select(x => x.Statut)),
                moi,
                prochaines,
                coworkers = charge,
                nonAttribues = ceMois.Count(x => x.Client.CoworkerId == null)
            });
        }

        private static DateOnly? Echeance(Client c, int annee, int mois)
        {
            var jour = c.GetJourTva(mois);
            if (jour == null) return null;

            return new DateOnly(annee, mois, Math.Min(jour.Value, DateTime.DaysInMonth(annee, mois)));
        }

        private static object Stats(IEnumerable<string> statuts)
        {
            var liste = statuts.ToList();

            return new
            {
                total = liste.Count,
                aValider = liste.Count(s => s == "avalider"),
                validees = liste.Count(s => s == "validee"),
                enRetard = liste.Count(s => s == "retard")
            };
        }
    }
}