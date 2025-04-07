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
            await client.PostAsync(url + "/appareil", content);
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
    
    //Fonction pour modifier un appareil
    public async static Task ModifyDevice(this MainWindow mainWindow, Device device)
    {
        var devicemod = device.ToAppareilDto();
        var serializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        try
        {
            string request = JsonSerializer.Serialize<AppareilDto>(devicemod, serializerOptions);
            JsonContent content = JsonContent.Create(device);
            await mainWindow.Client.PutAsync(mainWindow.ServerAddress + $"/appareil/{device.Id}", content);
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
    
    //Une fonction pour supprimer les appareils existants
    public async static void DeleteDevice(this MainWindow mainWindow, Device device)
    {
        try
        {
            await mainWindow.Client.DeleteAsync(mainWindow.ServerAddress + $"/appareil/{device.Id}");
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

    // Ajout de la méthode GetData avec paramètres de dates
    public static async Task<Data[]> GetData(this MainWindow mainWindow, HttpClient client, string url, Data[] currentData, uint id, DateTime startDate, DateTime endDate, JsonSerializerOptions? serializerOptions = null)
    {
        try
        {
            if (serializerOptions == default(JsonSerializerOptions))
            {
                serializerOptions = new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                };
            }

            try
            {
                // Format des dates pour l'API: yyyy-MM-ddTHH:mm:ss (format ISO 8601)
                string formattedStartDate = startDate.ToString("yyyy-MM-ddTHH:mm:ss");
                string formattedEndDate = endDate.ToString("yyyy-MM-ddTHH:mm:ss");
                
                // Création de la requête avec les paramètres de date
                string requestUrl = $"{url}/data/by-device/{id}?startDate={formattedStartDate}&endDate={formattedEndDate}";
                Console.WriteLine($"Requesting data from: {requestUrl}");
                
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseString))
                    {
                        Data[]? newData = JsonSerializer.Deserialize<Data[]>(responseString, serializerOptions);
                        
                        if (newData != null && newData.Length > 0)
                        {
                            return newData;
                        }
                        else
                        {
                            Console.WriteLine("No data found for the selected date range");
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
        return currentData;
    }
}
