namespace serveur.api.Dtos;
public record class CreateAppareilDto(
    string Nom,
    string Type,
    int Localisation,
    string Description
);
