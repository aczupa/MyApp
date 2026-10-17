using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new { email = User.Identity?.Name, isAdmin = User.IsInRole("Admin") });
        }
    }
}