using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhereToEat_BE.Models;
using WhereToEat_BE.Services;
using System.Security.Claims;

namespace WhereToEat_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AISuggestController : ControllerBase
    {
        private readonly PlacesService _placesService;
        private readonly AIService _aiService;

        public AISuggestController(PlacesService PlacesService, AIService aiService)
        {
            _placesService = PlacesService;
            _aiService = aiService;
        }
        [Authorize]
        [HttpPost("suggest")]
        public async Task <IActionResult> GetSuggestion([FromBody] SuggestionRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var places = await _placesService.GetRestaurants(request.Cuisine,request.Location);
            var suggestion = await _aiService.GetSuggestion(places, request.Cuisine, userId);
            return Ok(suggestion);
        }
    }
}