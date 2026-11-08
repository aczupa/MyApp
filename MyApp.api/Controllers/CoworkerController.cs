using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.api.Data;

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
    }
}