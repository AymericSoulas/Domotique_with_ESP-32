using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Client.Dtos;
using Client.Mapping;
using Client.Entities;


namespace Client.httpRequests;

public static class DataFunctions
{
    /*
     Fontion de récolte des données auprès de l'API
     */
    public static async Task<Data[]> GetData(this MainWindow mainWindow, HttpClient client, string url,
        Data[] data,
    DateTime beginTime = default(DateTime), DateTime endTime = default(DateTime))
    {
        try
        {
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            try
            {
                //On vérifie que des paramètres de temps ont été donnés, sinon on prend comme intervalle les deux derniers jours
                if (beginTime.Equals(default(DateTime)))
                {
                    beginTime = DateTime.Now.AddDays(-2);
                }
                if(endTime.Equals(default(DateTime)))
                {
                    endTime = DateTime.Now;
                }

                //Création de la requête pour récupérer les données auprès de l'API
                HttpResponseMessage response = await client.GetAsync(url + "/donnees/1/" +
                                                                     beginTime.ToString("yyyy-MM-dd:HH-mm") +
                                                                     "/" + endTime.ToString("yyyy-MM-dd:HH-mm"));
                Console.WriteLine(url + "/donnees/1/" +
                                  beginTime.AddDays(-2).ToString("yyyy-MM-dd:HH-mm") + "/" +
                                  endTime.ToString("yyyy-MM-dd:HH-mm"));

                //En cas de réponse
                if (response.IsSuccessStatusCode)
                {
                    //On récupère le contenu de la réponse
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Raw response for series: {responseContent}");

                    //On regarde si le contenu n'est pas vide
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        //On désérialise le contenu de la réponse pour le transférer dans des Dto connus
                        DonneeDto[]? donnees = JsonSerializer.Deserialize<DonneeDto[]>(responseContent, jsonOptions);

                        //Si des données ont été envoyées on efface les séries actuelles et on les remplaces par les nouvelles données.
                        if (donnees != null)
                        {
                            return donnees.ToEntity();
                        }
                        else
                        {
                            Console.WriteLine("Deserialized array is null");
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
        return data;
    }

    //Récolte de une donnée auprès de l'API
    public static async void GetOneData(this MainWindow mainWindow, HttpClient client, string url,
        JsonSerializerOptions jsonOptions, int dataId = 1)
    {
        try
        {
            Data temperature;
            //Création de la requête pour récupérer les données auprès de l'API
            HttpResponseMessage response = await client.GetAsync(url + "/donnees/" + dataId);
            Console.WriteLine(url + "/donnees/" + dataId);

            //En cas de réussite de la requête on lit le contenu de la réponse
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Raw response: {responseContent}");

                //Si le contenu n'est pas nul, on le désérialise pour le transférer dans un Dto connu
                if (!string.IsNullOrEmpty(responseContent))
                {
                    DonneeDto? donnees = JsonSerializer.Deserialize<DonneeDto>(responseContent, jsonOptions);

                    //Si des données ont été envoyées on les transfère dans l'objet Data et on actualise les éléments de 
                    if (donnees != null)
                    {
                        temperature = donnees.ToEntity();
                        Console.WriteLine(
                            $"Got data: Temperature={temperature.Temperature}, Humidity={temperature.Humidite}");
                    }
                    else
                    {
                        Console.WriteLine("Deserialized object is null");
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
            // You could display an error message to the user here
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
