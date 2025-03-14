using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using serveur.api.Entities;

namespace PostgreSQL.Data
{
    public class AppDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public AppDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // Importations des paramètres de connexion à la base de données depuis le fichier "appsettings.json"
            options.UseNpgsql(Configuration.GetConnectionString("WebApiDatabase"));
        }

        public DbSet<Donnees> donnees { get; set; }

        public DbSet<Appareil> appareils { get; set; }

        public DbSet<Piece> pieces { get; set; }
    }
}