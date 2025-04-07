namespace serveur.api.Dtos;

public record class UpdateAppareilDto(
    uint Id,
    string Nom,
    string Type,
    int Localisation,
    string? Description
);