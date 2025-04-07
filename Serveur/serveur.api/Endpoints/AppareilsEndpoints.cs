using Microsoft.EntityFrameworkCore;
using serveur.api.Dtos;
using PostgreSQL.Data;
using serveur.api.Entities;
using serveur.api.Mapping;

namespace serveur.api.Endpoints;

// Une classe gérant les endpoints pour les données
public static class AppareilsEndpoints
{
    public static RouteGroupBuilder MapAppareilEndpoints(this WebApplication app)
    {
        //Définition du groupe de routes pour les appareils
        var group = app.MapGroup("/appareil");

        //Message lors de l'accès à la route "racine" des appareils
        group.MapGet("/", () => "try putting an ID Behind");

        //Action lors de l'accès à un appareil unique
        group.MapGet("/{id}", (int id, AppDbContext dbContext) =>
        {
            Appareil? appareil = dbContext.appareils.Find(id);
            return appareil is null ? Results.NotFound() : Results.Ok(appareil);
        }).WithName("Get_individual_Device");
        
        group.MapGet("all", (AppDbContext dbContext) =>
        {
            Appareil[]? appareils = dbContext.appareils.ToArray();
            return appareils is null ? Results.NotFound() : Results.Ok(appareils);
        }).WithName("GetAll_Device");

        //Création d'un appareil
        group.MapPost("/", (CreateAppareilDto temp, AppDbContext dbContext) =>
        {
            Entities.Appareil appareil = temp.ToEntity();

            dbContext.appareils.Add(appareil);
            dbContext.SaveChanges();

            return Results.CreatedAtRoute("Get_individual_Device", new { Id = appareil.Id }, appareil.ToAppareilDto());
        });

        //Modification d'un appareil
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
        
        //Destruction d'un appareil
        group.MapDelete("/{id}", (uint id, AppDbContext dbContext) =>
        {
            dbContext.appareils.Where(appareil => appareil.Id == id).ExecuteDelete();
            
            return Results.NoContent();
        });

        //On retourne le groupe de routes
        return group;
    }
}

