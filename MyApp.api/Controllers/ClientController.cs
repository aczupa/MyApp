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
        public async Task<IActionResult> Get()
        {
            var clients = await _context.Clients.ToListAsync();

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
                c.RepresentantLegal
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
                RepresentantLegal = dto.RepresentantLegal
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
    }
}
