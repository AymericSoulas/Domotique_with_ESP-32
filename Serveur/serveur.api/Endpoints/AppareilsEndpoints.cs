using serveur.api.Dtos;
using PostgreSQL.Data;
using serveur.api.Entities;
using serveur.api.Mapping;

namespace serveur.api.Endpoints;

public static class AppareilsEndpoints
{
    public static RouteGroupBuilder MapAppareilEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/appareil");

        group.MapGet("/", () => "try putting an ID Behind");

        group.MapGet("/{id}", (int id, AppDbContext dbContext) =>
        {
            Appareil? appareil = dbContext.appareils.Find(id);
            return appareil is null ? Results.NotFound() : Results.Ok(appareil);
        }).WithName("Get_individual_Device");

        group.MapPost("/", (CreateAppareilDto temp, AppDbContext dbContext) =>
        {
            Entities.Appareil appareil = temp.ToEntity();

            dbContext.appareils.Add(appareil);
            dbContext.SaveChanges();

            return Results.CreatedAtRoute("Get_individual_Device", new { Id = appareil.Id }, appareil.ToAppareilDto());
        });

        group.MapPut("/{id}", (uint id, UpdateAppareilDto temp, AppDbContext dbContext) =>
        {
            Appareil? appareil = dbContext.appareils.Find(id);
            if (appareil is null)
            {
                return Results.NotFound();
            }

            dbContext.Entry(appareil).CurrentValues.SetValues(temp.ToEntity(id));
            dbContext.SaveChanges();
            return Results.Ok(appareil.ToAppareilDto());
        });

        return group;
    }
}

