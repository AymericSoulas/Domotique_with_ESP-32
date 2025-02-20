using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using PostgreSQL.Data;

namespace serveur.api.Data
{
    //Création de la classe d'extension pour la migration automatique au démarrage de la base de données
    public static class DataExtension
    {
        public static void MigrateDatabase(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Database.Migrate();
        }
    }
}
