using RestSharp;
using System.Text.Json;
using WhereToEat_BE.Models;

namespace WhereToEat_BE.Services
{
    public class PlacesService
    {
        private readonly IConfiguration _configuration;

        public PlacesService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<GooglePlacesResponse> GetRestaurants(string cuisine, string location, string? pageToken = null)
        {
            var client = new RestClient("https://places.googleapis.com");
            var request = new RestRequest("/v1/places:searchText");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-Goog-Api-Key", _configuration["Places:Secret"]);
            request.AddHeader("X-Goog-FieldMask", "places.displayName,places.formattedAddress,places.rating,places.priceLevel,places.primaryType,nextPageToken,places.googleMapsUri");

            object body;
            if (pageToken != null)
            {
                body = new { textQuery = $"{cuisine} in {location}", pageToken };
            }
            else
            {
                body = new { textQuery = $"{cuisine} in {location}" };
            }

            request.AddJsonBody(body);

            var response = await client.ExecutePostAsync(request);

            var googleResponse = JsonSerializer.Deserialize<GooglePlacesResponse>(response.Content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (googleResponse?.Places == null)
            {
                return new GooglePlacesResponse { Places = new List<GooglePlace>() };
            }

            return googleResponse;
        }
    }
}