using System.ComponentModel.DataAnnotations;

namespace MyApp.api.Models.DTOs
{
    public class CreateClientDto
    {
        [Required, StringLength(300)]
        public string DenominationSociale { get; set; } = string.Empty;
        public FormeJuridique FormeJuridique { get; set; }
        public string Adresse { get; set; } = string.Empty;
        public string Ville { get; set; } = string.Empty;
        public string CodePostal { get; set; } = string.Empty;
        public string Pays { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RepresentantLegal { get; set; } = string.Empty;

    }
}
