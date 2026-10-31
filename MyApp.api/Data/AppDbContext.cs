using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyApp.api.Models;

namespace MyApp.api.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<ValidationTva> ValidationsTva { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Client>()
                .Property(c => c.FormeJuridique)
                .HasConversion<string>();

            builder.Entity<Client>()
    .Property(c => c.PeriodiciteTva)
    .HasConversion<string>();

            builder.Entity<ValidationTva>()
    .HasIndex(v => new { v.ClientId, v.Annee, v.Mois })
    .IsUnique();
        }
    }
}