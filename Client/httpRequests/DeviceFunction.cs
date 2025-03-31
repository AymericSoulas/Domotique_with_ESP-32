using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Client.Dtos;
using Client.Entities;
using Client.Mapping;

namespace Client.httpRequests;

public static class DeviceFunction
{
    public static async Task<Device[]> GetDevice(this MainWindow mainWindow, HttpClient client, string url, JsonSerializerOptions? serializerOptions = null)
    {
        try
        {
            if (serializerOptions == default(JsonSerializerOptions))
            {
                serializerOptions = new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                };
            };
            try
            {
                //Création de la requête pour récupérer les données auprès de l'API
                HttpResponseMessage response = await client.GetAsync(url + "/appareil/all");
                Console.WriteLine(url + "/appareil/all");

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseString))
                    {
                        //On désérialise le contenu de la réponse pour le transférer dans des Dto connus
                        AppareilDto[]? devices = JsonSerializer.Deserialize<AppareilDto[]>(responseString, serializerOptions);

                        //Si des données ont été envoyées, on efface les séries actuelles et on les remplaces par les nouvelles données.
                        if (devices != null)
                        {
                            return devices.ToEntity();
                        }
                        else
                        {
                            Console.WriteLine("Deserialized array is null");
                            return new Device[]
                            {
                                new Device()
                                {
                                    Description = "Bonjour",
                                    Id = 1,
                                    Localisation = 0,
                                    Nom = "ESP-32 Version 1",
                                    Type = "ESP-32"
                                }
                            };
                        }
                    }
                    else
                    {
                        Console.WriteLine("Response was empty");
                    }
                }
                else
                {
                    Console.WriteLine($"Server returned error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request failed: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON parsing error: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Request timed out");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.GetType().Name} - {ex.Message}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Unexpected error: {e.Message}");
        }

        // Return the original data if we couldn't retrieve new data
        return null;
    }

    public async static void CreateDevice(this MainWindow mainWindow, HttpClient client, string url,
        CreateAppareilDto newdevice,  JsonSerializerOptions? serializerOptions = null)
    {
        if (serializerOptions == null)
        {
            serializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        };
        try
        {
            string request = JsonSerializer.Serialize<CreateAppareilDto>(newdevice, serializerOptions);
            JsonContent content = JsonContent.Create(newdevice);
            await client.PostAsync(url, content);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP request failed: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON parsing error: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Request timed out");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching data: {ex.GetType().Name} - {ex.Message}");
        }
    }
}