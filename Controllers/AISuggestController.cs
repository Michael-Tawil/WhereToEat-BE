using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhereToEat_BE.Models;
using WhereToEat_BE.Services;

namespace WhereToEat_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AISuggestController : ControllerBase
    {
        private readonly PlacesService _placesService;

        public AISuggestController(PlacesService PlacesService)
        {
            _placesService = PlacesService;
        }

        [HttpPost("suggest")]
        public async Task <IActionResult> GetSuggestion([FromBody] SuggestionRequest request)
        {
            var places = await _placesService.GetRestaurants(request.Cuisine,request.Location);
            return Ok(places);
        }
    }
}
