using RestSharp;
using System.Text.Json;
using WhereToEat_BE.Data;
using WhereToEat_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace WhereToEat_BE.Services
{
    public class AIService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AIService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public async Task<SuggestionResponse> GetSuggestion(List<GooglePlace> places, string cuisine, Guid UserId)
        {
            // 1. Build prompt from places list
            var lv = await _context.LastVisited.Where(l => l.UserId == UserId && l.Cuisine == cuisine).ToListAsync();
            var favs = await _context.Favourites.Where(l => l.UserId == UserId).ToListAsync();

            var lvText = lv.Any() ? string.Join(", ", lv.Select(l => l.RestaurantName)) : "None";
            var favsText = favs.Any() ? string.Join(", ", favs.Select(f => f.RestaurantName)) : "None";


            var placesText = string.Join("\n", places.Select(p =>
                $"- {p.DisplayName.Text}, {p.FormattedAddress}, Rating: {p.Rating}, Cuisine: {p.PrimaryType}, Price: {p.PriceLevel}"));

            var prompt = $@"You are a personalised restaurant recommendation assistant.

Here is a list of real restaurants:
{placesText}

The user is looking for: {cuisine} food.

User's favourite restaurants (suggest similar style if possible): {favsText}

Restaurants the user has recently visited (DO NOT suggest these): {lvText}

Rules:
- NEVER suggest a restaurant from the recently visited list
- NEVER suggest the same restaurant twice in a row
- If the user has favourites, prefer a similar style or cuisine
- Pick ONE restaurant the user hasn't been to recently
- Vary your suggestions each time — don't always pick the highest rated option
- Respond ONLY in JSON with exactly these fields:
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