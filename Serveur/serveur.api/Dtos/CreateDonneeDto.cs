namespace serveur.api.Dtos;
//Cr�ation du record permettant de cr�er des donn�es lors d'une requ�te POST
public record class CreateDonneeDto
(
    int Appareil,
    float Temperature,
    float Humidite
);
