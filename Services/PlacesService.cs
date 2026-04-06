using RestSharp;

namespace WhereToEat_BE.Services
{
    public class PlacesService
    {
        public async Task<string> GetRestaurants()
        {
            var client = new RestClient("https://places.googleapis.com");
            var request = new RestRequest("/v1/places:searchText");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-Goog-Api-Key", "AIzaSyBJFeAlKiWBjKEnz9kPkwBlS9j6F5yCUUU");
            request.AddHeader("X-Goog-FieldMask", "places.displayName,places.formattedAddress,places.rating");

            request.AddJsonBody(new
            {
                textQuery = "restaurants in Melbourne"
            });

            var response = await client.ExecutePostAsync(request);

            Console.WriteLine("Status: {0}", response.StatusCode);
            Console.WriteLine("Content: {0}", response.Content);

            return response.Content ?? "No content";
        }
    }
}