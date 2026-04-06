using RestSharp;
using System.Text.Json;
using WhereToEat_BE.Models;

namespace WhereToEat_BE.Services
{
    public class PlacesService
    {
        public async Task<List<GooglePlace>> GetRestaurants(string cuisine, string location)
        {
            var client = new RestClient("https://places.googleapis.com");
            var request = new RestRequest("/v1/places:searchText");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-Goog-Api-Key", "AIzaSyBJFeAlKiWBjKEnz9kPkwBlS9j6F5yCUUU");
            request.AddHeader("X-Goog-FieldMask", "places.displayName,places.formattedAddress,places.rating,places.priceLevel,places.primaryType");

            request.AddJsonBody(new
            {
                textQuery = $" {cuisine} restaurants in {location}"
            });

            var response = await client.ExecutePostAsync(request);

            if(response == null)
            {
                return new List<GooglePlace>();
            }
            else
            {
                var googleResponse = JsonSerializer.Deserialize<GooglePlacesResponse>(response.Content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return googleResponse.Places;
            }
        }
    }
}