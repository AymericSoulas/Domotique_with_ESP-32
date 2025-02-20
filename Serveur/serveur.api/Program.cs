using Microsoft.EntityFrameworkCore;

using serveur.api.Endpoints;
using PostgreSQL.Data;
using serveur.api.Data;
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("WebApiDatabase");

builder.Services.AddNpgsql<AppDbContext>(connectionString);

var app = builder.Build();

app.MapDonneeEndpoints();
app.MapPieceEndpoints();
app.MapAppareilEndpoints();

app.MapGet("/", () => "Hello World!");

app.MigrateDatabase();

app.Run();
