namespace Client.Entities;

public class Room
{
    public int Id { get; set; }
    public required string Nom { get; set; }
    public required string Localisation { get; set; }
}