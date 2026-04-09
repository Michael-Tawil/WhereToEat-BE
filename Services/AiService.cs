using RestSharp;
using System.Text.Json;
using WhereToEat_BE.Models;

namespace WhereToEat_BE.Services
{
    public class AIService
    {
        private readonly IConfiguration _configuration;

        public AIService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<SuggestionResponse> GetSuggestion(List<GooglePlace> places, string cuisine)
        {
            // 1. Build prompt from places list
            var placesText = string.Join("\n", places.Select(p =>
                $"- {p.DisplayName.Text}, {p.FormattedAddress}, Rating: {p.Rating}, Cuisine: {p.PrimaryType}, Price: {p.PriceLevel}"));

            var prompt = $@"You are a restaurant recommendation assistant.
Here is a list of real restaurants:
{placesText}

The user is looking for: {cuisine} food.
Pick ONE restaurant that best matches and respond ONLY in JSON with exactly these fields:
{{
  ""name"": ""restaurant name"",
  ""address"": ""full address"",
  ""rating"": 4.5,
  ""cuisine"": ""cuisine type"",
  ""priceRange"": ""$ or $$ or $$$ or $$$$"",
  ""reason"": ""why you picked this place""
}}
No extra text. Just the JSON.";

            // 2. Call Gemini API
            var client = new RestClient("https://generativelanguage.googleapis.com");
            var request = new RestRequest($"/v1beta/models/gemini-2.5-flash-lite:generateContent?key={_configuration["AI:Secret"]}");
            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            });

            var response = await client.ExecutePostAsync(request);

            Console.WriteLine("Gemini Status: {0}", response.StatusCode);
            Console.WriteLine("Gemini Content: {0}", response.Content);

            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(response.Content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Extract the text from the response
            var rawText = geminiResponse.Candidates[0].Content.Parts[0].Text;

            // Strip the ```json and ``` markers Gemini adds
            var cleanJson = rawText.Replace("```json", "").Replace("```", "").Trim();

            // Deserialize into your SuggestionResponse
            var suggestion = JsonSerializer.Deserialize<SuggestionResponse>(cleanJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return suggestion;
        }
    }
}