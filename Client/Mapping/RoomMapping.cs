using Client.Dtos;
using Client.Entities;

namespace DomoTiX.Mapping;

public static class RoomMapping
{
    public static Room ToEntity(this CreatePieceDto newpiece)
    {
        return new Room()
        {
            Nom = newpiece.Nom,
            Localisation = newpiece.Localisation
        };
    }

    public static PieceDto ToPieceDto(this Room piece)
    {
        return new
        (
            Id: piece.Id,
            Nom: piece.Nom,
            Localisation: piece.Localisation
        );
    }

    public static Room ToEntity(this UpdatePieceDto newpiece, int tempId)
    {
        return new Room()
        {
            Id = tempId,
            Nom = newpiece.Nom,
            Localisation = newpiece.Localisation
        };
    }
}