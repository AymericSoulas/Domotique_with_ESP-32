namespace Client.Entities
{
    public class Device
    {
        public uint Id { get; set; }
        public required string Nom { get; set; }
        public required string Type { get; set; }
        public required int Localisation { get; set; }
        public string? Description { get; set; }
    }
}
