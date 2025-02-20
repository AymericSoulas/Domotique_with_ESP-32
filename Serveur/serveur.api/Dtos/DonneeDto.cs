namespace serveur.api.Dtos;

//Cr�ation du record permettant de r�cup�rer des donn�es lors d'une requ�te GET
public record class DonneeDto
(
    uint Id,
    string Date,
    int Appareil,
    float Temperature,
    float Humidite
);
