namespace Client.Dtos;
public record class AppareilDto(
    uint Id,
    string Nom,
    string Type,
    int Localisation,
    string? Description
);