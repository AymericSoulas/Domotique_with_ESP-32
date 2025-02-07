using serveur.api.Dtos;

namespace serveur.api.Endpoints;

public static class TemperatureEndpoints{
    private static readonly List<TemperatureDto> temperatures = [
        new (1, DateTime.Now.ToString("yyyy-MM-dd HH:mm"), "ESP32-1", "Salon", 20.0f, 0.8f),
        new (2, DateTime.Now.ToString("yyyy-MM-dd HH:mm"), "ESP32-2", "Chambre", 21.0f, 0.7f),
        new (3, DateTime.Now.ToString("yyyy-MM-dd HH:mm"), "ESP32-3", "Cuisine", 22.0f, 0.6f),
        new (4, DateTime.Now.ToString("yyyy-MM-dd HH:mm"), "ESP32-4", "Salle de bain", 23.0f, 0.5f),
        new (5, DateTime.Now.ToString("yyyy-MM-dd HH:mm"), "ESP32-5", "Toilettes", 24.0f, 0.4f)
    ];
    public static RouteGroupBuilder MapTemperatureEndpoints(this WebApplication app){
    

        var group = app.MapGroup("/temperature");

        group.MapGet("/", () => temperatures);

        group.MapGet("/{id}", (int id) => temperatures.FirstOrDefault(t => t.Id == id))
                .WithName("Get_individual_temperature");

        group.MapPost("/", (CreateTemperatureDto temp) => {
            TemperatureDto temperature = new (
                temperatures.Max(t => t.Id) + 1,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                temp.Appareil,
                temp.Piece,
                temp.Temperature,
                temp.Humidite
            );
            temperatures.Add(temperature);
            return Results.CreatedAtRoute("Get_individual_temperature", new { Id = temperature.Id }, temperature);
        });
        
        return group;
    }
    
}