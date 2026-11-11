using System.ComponentModel.DataAnnotations;

namespace MyApp.api.Models
{
    public enum RegimeFiscal
    {
        [Display(Name = "Non renseigné")]
        NonRenseigne,
        [Display(Name = "Micro-entreprise")]
        MicroEntreprise,
        [Display(Name = "Réel simplifié")]
        ReelSimplifie,
        [Display(Name = "Réel normal")]
        ReelNormal
    }
}