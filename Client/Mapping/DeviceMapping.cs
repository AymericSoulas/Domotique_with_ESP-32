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

    public static Device ToEntity(this AppareilDto newdevice)
    {
        return new Device()
        {
            Id = newdevice.Id,
            Nom = newdevice.Nom,
            Type = newdevice.Type,
            Localisation = newdevice.Localisation,
            Description = newdevice.Description
        };
    }
    
    public static Device[] ToEntity(this AppareilDto[] newdevices)
    {
        Device[] devices = new Device[newdevices.Length];
        for (int i = 0; i < newdevices.Length; i++)
        {
            devices[i] = newdevices[i].ToEntity();
        }
        return devices;
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