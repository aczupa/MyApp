using System.ComponentModel.DataAnnotations;

namespace MyApp.api.Models
{
    public enum PeriodiciteTva
    {
        [Display(Name = "Non concerné")]
        NonConcerne,
        [Display(Name = "Mensuelle")]
        Mensuelle,
        [Display(Name = "Trimestrielle")]
        Trimestrielle
    }
}
