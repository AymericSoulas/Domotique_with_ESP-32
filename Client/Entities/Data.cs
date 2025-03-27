using System;
using System.Collections.ObjectModel;
using LiveChartsCore;

namespace Client.Entities;

public class Data
{
    public uint Id { get; set; }
    public required DateTime Date { get; set; }
    public required int Appareil { get; set; }
    public required float Temperature { get; set; }
    public required float Humidite { get; set; }
}