using System.Runtime.CompilerServices;
using serveur.api.Dtos;
using serveur.api.Entities;

namespace serveur.api.Mapping;

public static class AppareilMapping
{
    public static Appareil ToEntity(this CreateAppareilDto newdevice)
    {
        return new Appareil()
        {
            Nom = newdevice.Nom,
            Type = newdevice.Type,
            Localisation = newdevice.Localisation,
            Description = newdevice.Description
        };
    }

    public static AppareilDto ToAppareilDto(this Appareil appareil)
    {
        return new
        (
            Id: appareil.Id,
            Nom: appareil.Nom,
            Type:  appareil.Type,
            Localisation: appareil.Localisation,
            Description: appareil.Description
        );
    }

    public static Appareil ToEntity(this UpdateAppareilDto newdevice, uint id)
    {
        return new Appareil()
        {
            Id = id,
            Nom = newdevice.Nom,
            Type = newdevice.Type,
            Localisation = newdevice.Localisation,
            Description = newdevice.Description
        };
    }
}