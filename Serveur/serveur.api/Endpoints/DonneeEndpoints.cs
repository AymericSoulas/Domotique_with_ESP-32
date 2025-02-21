using serveur.api.Dtos;
using PostgreSQL.Data;
using serveur.api.Entities;
using serveur.api.Mapping;

namespace serveur.api.Endpoints;

public static class DonneesEndpoints
{
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