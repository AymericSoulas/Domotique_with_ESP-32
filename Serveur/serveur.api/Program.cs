using Microsoft.EntityFrameworkCore;

using serveur.api.Endpoints;
using PostgreSQL.Data;
using serveur.api.Data;


var builder = WebApplication.CreateBuilder(args);

//Configuration de la base de donn�es et ajout du service � l'API
var connectionString = builder.Configuration.GetConnectionString("WebApiDatabase");
builder.Services.AddNpgsql<AppDbContext>(connectionString);

//Construction de l'application
var app = builder.Build();

//Application des diff�rents endpoints d�finis dans les fichiers situ�s sous /Endpoints
app.MapDonneeEndpoints();
app.MapPieceEndpoints();
app.MapAppareilEndpoints();

//D�finition de la route racine par d�faut (sera peut-�tre chang�e pour prendre en charge une interface web)
app.MapGet("/", () => "Hello World!");

//Migration automatique de la base de donn�es au d�marrage de l'application
app.MigrateDatabase();

//Lancement de l'API
app.Run();
