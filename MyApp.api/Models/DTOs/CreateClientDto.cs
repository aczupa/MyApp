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
        public int? CoworkerId { get; set; }

        public PeriodiciteTva PeriodiciteTva { get; set; }

        [Range(1, 31)]
        public int? JourTva { get; set; }

        public ModeImposition ModeImposition { get; set; }
        public RegimeFiscal RegimeFiscal { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (PeriodiciteTva != PeriodiciteTva.NonConcerne && JourTva == null)
                yield return new ValidationResult("Le jour de TVA est obligatoire.", new[] { nameof(JourTva) });

            var monaco = FormeJuridique.EstMonegasque();

            if (monaco && (ModeImposition is ModeImposition.IR or ModeImposition.IS))
                yield return new ValidationResult("Un client monégasque relève de l'ISB, pas de l'IR ou de l'IS.", new[] { nameof(ModeImposition) });

            if (!monaco && (ModeImposition is ModeImposition.ISB or ModeImposition.NonSoumisIsb))
                yield return new ValidationResult("L'ISB ne concerne que les sociétés monégasques.", new[] { nameof(ModeImposition) });
        }

      

       
    }
}