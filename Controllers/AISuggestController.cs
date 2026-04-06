using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task <IActionResult> GetSuggestion()
        {
            var result = await _placesService.GetRestaurants();
            return Ok(result);
        }
    }
}
