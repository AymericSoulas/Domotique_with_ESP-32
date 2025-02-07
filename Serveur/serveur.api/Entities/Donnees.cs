namespace serveur.api.Entities
{
    public class Donnees{
        public required uint Id { get; set; }
        public required string Date { get; set; }
        public required string Appareil { get; set; }
        public required string Piece { get; set; }
        public required float Temperature { get; set; }
        public required float Humidite { get; set; }
    }
}
