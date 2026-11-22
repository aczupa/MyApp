using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.api.Data;
using MyApp.api.Models;

namespace MyApp.api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CoworkerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CoworkerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var coworkers = await _context.Coworkers
                .OrderBy(c => c.Nom)
                .Select(c => new { c.Id, c.Nom, NbClients = c.Clients.Count })
                .ToListAsync();

            return Ok(coworkers);
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var w = await _context.Coworkers
                .Include(c => c.User)
                .Include(c => c.Clients)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (w == null) return NotFound();

            return Ok(new
            {
                w.Id,
                w.Nom,
                Email = w.User?.Email,
                Clients = w.Clients
                    .OrderBy(c => c.DenominationSociale)
                    .Select(c => new
                    {
                        c.Id,
                        c.DenominationSociale,
                        FormeJuridique = c.FormeJuridique.GetDisplayName(),
                        c.Ville,
                        Tva = c.PeriodiciteTva == PeriodiciteTva.NonConcerne
                            ? c.PeriodiciteTva.GetDisplayName()
                            : $"{c.PeriodiciteTva.GetDisplayName()}, le {c.JourTva}"
                    })
            });
        }
    }
}