namespace serveur.api.Dtos;
//Création du record permettant de créer des données lors d'une requête POST
public record class CreateDonneeDto
(
    int Appareil,
    float Temperature,
    float Humidite
);
