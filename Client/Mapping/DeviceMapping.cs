using System.Runtime.CompilerServices;
using Client.Dtos;
using Client.Entities;

namespace Client.Mapping;

public static class DeviceMapping
{
    public static Device ToEntity(this CreateAppareilDto newdevice)
    {
        return new Device()
        {
            Nom = newdevice.Nom,
            Type = newdevice.Type,
            Localisation = newdevice.Localisation,
            Description = newdevice.Description
        };
    }

    public static AppareilDto ToAppareilDto(this Device appareil)
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

    public static Device ToEntity(this UpdateAppareilDto newdevice, uint id)
    {
        return new Device()
        {
            Id = id,
            Nom = newdevice.Nom,
            Type = newdevice.Type,
            Localisation = newdevice.Localisation,
            Description = newdevice.Description
        };
    }
}