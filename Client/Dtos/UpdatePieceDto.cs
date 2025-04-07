namespace Client.Dtos;
public record class UpdatePieceDto(
    uint Id,
    string Nom,
    string Localisation
);