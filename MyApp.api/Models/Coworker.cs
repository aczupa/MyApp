using Microsoft.AspNetCore.Identity;

namespace MyApp.api.Models
{
    public class Coworker
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        public IdentityUser? User { get; set; }

        public List<Client> Clients { get; set; } = new();
    }
}
