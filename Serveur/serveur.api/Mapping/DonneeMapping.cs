using System.Runtime.CompilerServices;
using serveur.api.Dtos;
using serveur.api.Entities;

namespace serveur.api.Mapping;

public static class DonneeMapping
{
    public static Donnees ToEntity(this CreateDonneeDto newdata)
    {
        return new Donnees()
        {
            Date = DateTime.Now.ToString("yyyy-MM-dd:HH-mm"),
            Appareil = newdata.Appareil,
            Temperature = newdata.Temperature,
            Humidite = newdata.Humidite
        };
    }

    public static DonneeDto ToAppareilDto(this Donnees data)
    {
        return new
        (
            Id: data.Id,
            Appareil: data.Appareil,
            Date: data.Date,
            Temperature: data.Temperature,
            Humidite: data.Humidite
        );
    }
}