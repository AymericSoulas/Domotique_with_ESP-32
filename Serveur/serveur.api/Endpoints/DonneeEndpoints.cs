using serveur.api.Dtos;
using PostgreSQL.Data;
using serveur.api.Entities;
using serveur.api.Mapping;

namespace serveur.api.Endpoints;

public static class DonneesEndpoints
{
    private static readonly List<DonneeDto> temperatures = [
        new (1, DateTime.Now.ToString("yyyy-MM-dd HH:mm"), 0, 20.0f, 0.8f)
    ];
    public static RouteGroupBuilder MapDonneeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/donnees");

        group.MapGet("/", () => "try putting an ID Behind");

        group.MapGet("/{id}", (uint id, AppDbContext dbContext) =>
        {
            Donnees? donnee = dbContext.donnees.Find(id);
            return donnee is null ? Results.NotFound() : Results.Ok(donnee);
        }).WithName("Get_individual_data");

        group.MapPost("/", (CreateDonneeDto temp, AppDbContext dbContext) =>
        {
            Entities.Donnees temperature = temp.ToEntity();
            dbContext.donnees.Add(temperature);
            dbContext.SaveChanges();

            return Results.CreatedAtRoute("Get_individual_Data", new { Id = temperature.Id }, temperature.ToAppareilDto());
        });

        return group;
    }

}