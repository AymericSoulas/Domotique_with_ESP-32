using serveur.api.Dtos;
using PostgreSQL.Data;
using serveur.api.Entities;
using serveur.api.Mapping;
using System.Globalization;

namespace serveur.api.Endpoints;


// Une classe gérant les endpoints pour les données
public static class DonneesEndpoints
{
    public static RouteGroupBuilder MapDonneeEndpoints(this WebApplication app)
    {
        //Définition du groupe de routes pour les données
        var group = app.MapGroup("/donnees");

        //Message lors de l'accès à la route "racine" des donnees
        group.MapGet("/", () => "try putting an ID Behind");

        //Action l'ors de l'accès à une donnée unique
        group.MapGet("/{id}", (uint id, AppDbContext dbContext) =>
        {
            Donnees? donnee = dbContext.donnees.Find(id);
            return donnee is null ? Results.NotFound() : Results.Ok(donnee);
        }).WithName("Get_individual_data");

        //Accès à un groupe de données selon l'appareil et un intervale de dates
        group.MapGet("/{deviceid}/{BeginDate}/{EndDate}", (uint deviceid, string BeginDate, string EndDate, AppDbContext dbContext) =>
        {
            DateTime beginDateTime;
            DateTime endDateTime;
            try
            {
                beginDateTime = DateTime.SpecifyKind(DateTime.ParseExact(BeginDate, "yyyy-MM-dd:HH-mm", CultureInfo.InvariantCulture), DateTimeKind.Utc);
                endDateTime = DateTime.SpecifyKind(DateTime.ParseExact(EndDate, "yyyy-MM-dd:HH-mm", CultureInfo.InvariantCulture), DateTimeKind.Utc);
            }
            catch (FormatException)
            {
                return Results.BadRequest("Les dates doivent être au format 'yyyy-MM-dd:HH-mm'.");
            }
            List<Donnees> donnees = dbContext.donnees
            .Where(d => d.Appareil == deviceid && d.Date >= beginDateTime && d.Date <=endDateTime)
            .ToList();
            return donnees is null ? Results.NotFound() : Results.Ok(donnees);
        }).WithName("Get_data_by_device_and_date");

        //Création d'une donnée (route utilisée par les capteurs)
        group.MapPost("/", (CreateDonneeDto temp, AppDbContext dbContext) =>
        {
            Entities.Donnees temperature = temp.ToEntity();
            dbContext.donnees.Add(temperature);
            dbContext.SaveChanges();

            return Results.CreatedAtRoute("Get_individual_Data", new { Id = temperature.Id }, temperature.ToAppareilDto());
        });

        //On retourne le groupe de routes
        return group;
    }

}