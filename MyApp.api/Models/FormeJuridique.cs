using System.ComponentModel.DataAnnotations;

namespace MyApp.api.Models
{
    public enum FormeJuridique
    {
        // France
        [Display(Name = "Entreprise individuelle")]
        EntrepriseIndividuelle,
        [Display(Name = "Micro-entreprise")]
        MicroEntreprise,
        [Display(Name = "EURL")]
        EURL,
        [Display(Name = "SARL")]
        SARL,
        [Display(Name = "SASU")]
        SASU,
        [Display(Name = "SAS")]
        SAS,
        [Display(Name = "SA")]
        SA,
        [Display(Name = "SNC")]
        SNC,
        [Display(Name = "SCS")]
        SCS,
        [Display(Name = "SCA")]
        SCA,
        [Display(Name = "Association loi 1901")]
        Association1901,
        [Display(Name = "SCOP")]
        SCOP,
        [Display(Name = "SCI")]
        SCI,

        // Monaco
        [Display(Name = "Entreprise individuelle Monaco")]
        EntrepriseIndividuelleMonaco,
        [Display(Name = "SARL Monaco")]
        SARLMonaco,
        [Display(Name = "SAM Société anonyme monégasque")]
        SAM,
        [Display(Name = "SNC Société en nom collectif Monaco")]
        SNCMonaco,
        [Display(Name = "SCS Société en commandite simple Monaco")]
        SCSMonaco,
        [Display(Name = "SC Société civile Monaco")]
        SocieteCivileMonaco
    }
}