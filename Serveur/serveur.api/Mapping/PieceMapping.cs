using System.Runtime.CompilerServices;
using serveur.api.Dtos;
using serveur.api.Entities;

namespace serveur.api.Mapping;

public static class PieceMapping
{
    public static Piece ToEntity(this CreatePieceDto newpiece)
    {
        return new Piece()
        {
            Nom = newpiece.Nom,
            Localisation = newpiece.Localisation
        };
    }

    public static PieceDto ToPieceDto(this Piece piece)
    {
        return new
        (
            Id: piece.Id,
            Nom: piece.Nom,
            Localisation: piece.Localisation
        );
    }

    public static Piece ToEntity(this UpdatePieceDto newpiece, int tempId)
    {
        return new Piece()
        {
            Id = tempId,
            Nom = newpiece.Nom,
            Localisation = newpiece.Localisation
        };
    }
}