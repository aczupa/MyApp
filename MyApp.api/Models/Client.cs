namespace MyApp.api.Models
{
    public class Client
    {
        public int Id { get; set; }
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
        public int? JourTva { get; set; }

        private static readonly int[] MoisTrimestriel = { 1, 4, 7, 10 };

        public int? GetJourTva(int mois) => PeriodiciteTva switch
        {
            PeriodiciteTva.Mensuelle => JourTva,
            PeriodiciteTva.Trimestrielle => MoisTrimestriel.Contains(mois) ? JourTva : null,
            _ => null
        };
    }
}