namespace serveur.api.Dtos;

public record class TemperatureDto
(
    uint Id,
    string Date,
    string Appareil,
    string Piece,
    float Temperature,
    float Humidite
);
