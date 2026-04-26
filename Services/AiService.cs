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
        private readonly PlacesService _placesService;

        public AIService(IConfiguration configuration, AppDbContext context, PlacesService placesService)
        {
            _configuration = configuration;
            _context = context;
            _placesService = placesService;
        }

        public async Task<SuggestionResponse> GetSuggestion(GooglePlacesResponse placesResponse, string cuisine, string location, Guid UserId)
        {
            // Fetch user history
            var lv = await _context.LastVisited.Where(l => l.UserId == UserId).ToListAsync();
            var favs = await _context.Favourites.Where(l => l.UserId == UserId).ToListAsync();
            var suggested = await _context.Suggested.Where(s => s.UserId == UserId).ToListAsync();

            // Start with first page
            var allPlaces = new List<GooglePlace>(placesResponse.Places);
            var nextPageToken = placesResponse.NextPageToken;

            // Keep fetching pages until we have 5+ unseen places
            while (nextPageToken != null)
            {
                var remaining = allPlaces
                    .Where(p => !suggested.Any(s => s.RestaurantName == p.DisplayName.Text))
                    .ToList();

                if (remaining.Count >= 5) break;

                var nextPage = await _placesService.GetRestaurants(cuisine, location, nextPageToken);
                allPlaces.AddRange(nextPage.Places);
                nextPageToken = nextPage.NextPageToken;
            }

            // Get final unseen pool
            var pool = allPlaces
                .Where(p => !suggested.Any(s => s.RestaurantName == p.DisplayName.Text))
                .ToList();


            if (!pool.Any())
            {
                return new SuggestionResponse { Name = "No suggestions available", Reason = "You've seen all available restaurants in this area. Try a different location or cuisine." };
            }

            // Build texts for prompt
            var lvText = lv.Any() ? string.Join(", ", lv.Select(l => l.RestaurantName)) : "None";
            var favsText = favs.Any() ? string.Join(", ", favs.Select(f => f.RestaurantName)) : "None";
            var suggestedText = suggested.Any() ? string.Join(", ", suggested.Select(s => s.RestaurantName)) : "None";

            var placesText = string.Join("\n", pool.Select(p =>
                $"- {p.DisplayName.Text}, {p.FormattedAddress}, Rating: {p.Rating}, Cuisine: {p.PrimaryType}, Price: {p.PriceLevel}"));

            var prompt = $@"You are a personalised dining and food spot recommendation assistant.

Here is a list of real places:
{placesText}

The user is looking for: {cuisine} in {location}

User's favourites (understand their taste from these): {favsText}
Places already suggested (DO NOT suggest these): {suggestedText}
Places recently visited (DO NOT suggest these): {lvText}

Rules:
- NEVER suggest a place from the already suggested or recently visited lists
- The user's request may be a cuisine, a vibe, a type of food, or a description — interpret it intelligently
- Use the favourites to understand the user's taste preferences
- Pick ONE place that best matches the user's intent
- Vary your suggestions — don't always pick the highest rated
- Respond ONLY in JSON with exactly these fields:
{{
  ""name"": ""place name"",
  ""address"": ""full address"",
  ""rating"": 4.5,
  ""cuisine"": ""type of place"",
  ""priceRange"": ""$ or $$ or $$$ or $$$$"",
  ""reason"": ""why you picked this place""
}}
No extra text. Just the JSON.";

            // Call Gemini API
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

            if (geminiResponse?.Candidates == null || geminiResponse.Candidates.Count == 0)
            {
                throw new Exception($"Gemini returned no candidates. Response: {response.Content}");
            }

            var rawText = geminiResponse.Candidates[0].Content.Parts[0].Text;
            var cleanJson = rawText.Replace("```json", "").Replace("```", "").Trim();

            var suggestion = JsonSerializer.Deserialize<SuggestionResponse>(cleanJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var matchedPlace = pool.FirstOrDefault(p => p.DisplayName.Text.Equals(suggestion.Name, StringComparison.OrdinalIgnoreCase));

            if (matchedPlace != null)
            {
                suggestion.GoogleMapsUri = matchedPlace.GoogleMapsUri;
            }

            // Save suggestion to history
            _context.Suggested.Add(new Suggested
            {
                UserId = UserId,
                RestaurantName = suggestion.Name
            });
            await _context.SaveChangesAsync();

            return suggestion;
        }
    }
}