using System.ComponentModel.DataAnnotations;

namespace MyApp.api.Models.DTOs
{
    public class CreateClientDto : IValidatableObject
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

        public PeriodiciteTva PeriodiciteTva { get; set; }

        [Range(1, 31)]
        public int? JourTva { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (PeriodiciteTva != PeriodiciteTva.NonConcerne && JourTva == null)
                yield return new ValidationResult("Le jour de TVA est obligatoire.", new[] { nameof(JourTva) });
        }
    }
}