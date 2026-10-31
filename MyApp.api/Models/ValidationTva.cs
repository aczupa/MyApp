namespace MyApp.api.Models
{
    public class ValidationTva
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public int Annee { get; set; }
        public int Mois { get; set; }
        public DateTime DateValidation { get; set; }
    }
}
