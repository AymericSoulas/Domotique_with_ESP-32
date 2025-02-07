namespace serveur.api.Dtos;

public record class CreateTemperatureDto
(
    string Appareil,
    string Piece,
    float Temperature,
    float Humidite
);
