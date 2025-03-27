namespace Client.Dtos;
public record class CreateAppareilDto(
    string Nom,
    string Type,
    int Localisation,
    string Description
);
