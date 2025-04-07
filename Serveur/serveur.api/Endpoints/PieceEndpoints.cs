using Microsoft.EntityFrameworkCore;
using PostgreSQL.Data;
using serveur.api.Dtos;
using serveur.api.Entities;
using serveur.api.Mapping;

namespace serveur.api.Endpoints;

// Une classe gérant les endpoints pour les Pièces
public static class PieceEndpoints
{
    public static RouteGroupBuilder MapPieceEndpoints(this WebApplication app)
    {
        //Définition du groupe de routes pour les Pièces
        var group = app.MapGroup("/Piece");

        //Message lors de l'accès à la route "racine" des Pièces
        group.MapGet("/", () => "try putting an ID Behind");
        
        //Récupération de la totalité des pièces enregistrées dans la base de données
        group.MapGet("all", (AppDbContext dbContext) =>
        {
            Piece?[] piece = dbContext.pieces.ToArray();
            return piece is null ? Results.NoContent() : Results.Ok(piece);
        }).WithName("GetAllPieces");

        //Action lors de l'accès à une Pièce unique
        group.MapGet("/{id}", (int id, AppDbContext dbContext) =>
        {
            Piece? piece = dbContext.pieces.Find(id);
            return piece is null ? Results.NotFound() : Results.Ok(piece);
        }).WithName("Get_individual_Piece");

        //Création d'une Pièce
        group.MapPost("/", (CreatePieceDto temp, AppDbContext dbContext) =>
        {
            Piece piece = temp.ToEntity();

            dbContext.pieces.Add(piece);
            dbContext.SaveChanges();

            return Results.CreatedAtRoute("Get_individual_Piece", new { Id = piece.Id }, piece.ToPieceDto());
        });

        //Modification d'une Pièce
        group.MapPut("/{id}", (int id, UpdatePieceDto temp, AppDbContext dbContext) =>
        {
            Piece? piece = dbContext.pieces.Find(id);
            if (piece is null)
            {
                return Results.NotFound();
            }
            
            dbContext.Entry(piece).CurrentValues.SetValues(temp.ToEntity(id));

            dbContext.SaveChanges();
            return Results.Ok(piece.ToPieceDto());
        });
        
        //Suppression d'une pièce
        group.MapDelete("/{id}", (int id, AppDbContext dbContext) =>
        {
            dbContext.pieces.Where(piece => piece.Id == id).ExecuteDelete();
            return Results.NoContent();
        });

        //On retourne le groupe de routes
        return group;
    }
}

