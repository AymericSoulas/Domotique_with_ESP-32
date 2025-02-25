using Microsoft.EntityFrameworkCore;

using serveur.api.Endpoints;
using PostgreSQL.Data;
using serveur.api.Data;


var builder = WebApplication.CreateBuilder(args);

//Configuration de la base de données et ajout du service à l'API
var connectionString = builder.Configuration.GetConnectionString("WebApiDatabase");
builder.Services.AddNpgsql<AppDbContext>(connectionString);

//Construction de l'application
var app = builder.Build();

//Application des différents endpoints définis dans les fichiers situés sous /Endpoints
app.MapDonneeEndpoints();
app.MapPieceEndpoints();
app.MapAppareilEndpoints();

//Définition de la route racine par défaut (sera peut-être changée pour prendre en charge une interface web)
app.MapGet("/", () => "Hello World!");

//Migration automatique de la base de données au démarrage de l'application
app.MigrateDatabase();

//Lancement de l'API
app.Run();
