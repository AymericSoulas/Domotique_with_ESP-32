using serveur.api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapTemperatureEndpoints();

app.MapGet("/", () => "Hello World!");

app.Run();
