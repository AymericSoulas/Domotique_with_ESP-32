using System;
using System.Text.Json.Serialization;

namespace Client.Dtos;

//Création du record permettant de récupérer des données lors d'une requête GET
public record class DonneeDto
(
    [property: JsonPropertyName("id")] uint Id,
    [property: JsonPropertyName("date")] DateTime Date,
    [property: JsonPropertyName("appareil")] int Appareil,
    [property: JsonPropertyName("temperature")] float Temperature,
    [property: JsonPropertyName("humidite")] float Humidite
);
