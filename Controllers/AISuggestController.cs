using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WhereToEat_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AISuggestController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetSuggestion() => Ok(new[] { "test from API" });
    }
}
