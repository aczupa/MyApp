using System.ComponentModel.DataAnnotations;

namespace MyApp.api.Models
{
    public enum ModeImposition
    {
        [Display(Name = "Non renseigné")]
        NonRenseigne,
        [Display(Name = "Impôt sur le revenu (IR)")]
        IR,
        [Display(Name = "Impôt sur les sociétés (IS)")]
        IS,
        [Display(Name = "Impôt sur les bénéfices (ISB)")]
        ISB,
        [Display(Name = "Non soumis à l'ISB")]
        NonSoumisIsb
    }
}