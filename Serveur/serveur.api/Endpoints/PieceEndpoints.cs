using PostgreSQL.Data;
using serveur.api.Dtos;
using serveur.api.Entities;
using serveur.api.Mapping;

public static class PieceEndpoints
{
    public static RouteGroupBuilder MapPieceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/Piece");

        group.MapGet("/", () => "try putting an ID Behind");

        group.MapGet("/{id}", (int id, AppDbContext dbContext) =>
        {
            Piece? piece = dbContext.pieces.Find(id);
            return piece is null ? Results.NotFound() : Results.Ok(piece);
        }).WithName("Get_individual_Piece");

        group.MapPost("/", (CreatePieceDto temp, AppDbContext dbContext) =>
        {
            Piece piece = temp.ToEntity();

            dbContext.pieces.Add(piece);
            dbContext.SaveChanges();

            return Results.CreatedAtRoute("Get_individual_Piece", new { Id = piece.Id }, piece.ToPieceDto());
        });
        

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
        return group;
    }
}

