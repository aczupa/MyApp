using System.ComponentModel.DataAnnotations;

namespace MyApp.api.Models.DTOs
{
    public class ValidationTvaDto
    {

        [Range(2000, 2100)]
        public int Annee { get; set; }

        [Range(1, 12)]
        public int Mois { get; set; }

        public bool Validee { get; set; }
      
    }
}
