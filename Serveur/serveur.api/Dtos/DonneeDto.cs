namespace serveur.api.Dtos;

//Création du record permettant de récupérer des données lors d'une requête GET
public record class DonneeDto
(
    uint Id,
    string Date,
    int Appareil,
    float Temperature,
    float Humidite
);
