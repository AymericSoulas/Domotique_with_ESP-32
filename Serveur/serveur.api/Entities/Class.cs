namespace serveur.api.Entities
{
    public class Appareil{
        public uint Id { get; set; }
        public required string Nom { get; set; }
        public required string Type { get; set; }
        public required string Localisation { get; set; }
        public string? Description { get; set; }
    }
}
