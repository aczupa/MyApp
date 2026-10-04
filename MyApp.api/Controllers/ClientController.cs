using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var client = new[]
            {
                new { Id = 1, Name = "John Doe", Statut = "Private" },
                new { Id = 2, Name = "Smith", Statut = "Enterprise" }
            };

            return Ok(client);
        }
    }
}
