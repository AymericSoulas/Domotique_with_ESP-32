using System;
using Client.Dtos;
using Client.Entities;

namespace Client.Mapping;

public static class DataMapping
{
    public static Data ToEntity(this CreateDonneeDto newdata)
    {
        return new Data()
        {
            Date = DateTime.UtcNow.AddHours(1),
            Appareil = newdata.Appareil,
            Temperature = newdata.Temperature,
            Humidite = newdata.Humidite
        };
    }
    
    public static Data ToEntity(this DonneeDto newdata)
    {
        return new Data()
        {
            Id = newdata.Id,
            Date = newdata.Date,
            Appareil = newdata.Appareil,
            Temperature = newdata.Temperature,
            Humidite = newdata.Humidite
        };
    }
    
    public static Data[] ToEntity(this DonneeDto[] newdata)
    {
        Data[] data = new Data[newdata.Length];
        
        for (int i = 0; i < newdata.Length; i++)
        {
            data[i] = newdata[i].ToEntity();
        }
        
        return data;
    }

    public static DonneeDto ToAppareilDto(this Data data)
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