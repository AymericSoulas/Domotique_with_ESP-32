namespace Client.Dtos;
//Création du record permmettant de cr�er des donn�es lors d'une requète POST
public record class CreateDonneeDto
(
    int Appareil,
    float Temperature,
    float Humidite
);
