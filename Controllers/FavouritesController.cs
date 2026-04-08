using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using WhereToEat_BE.Data;
using WhereToEat_BE.Models;

namespace WhereToEat_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavouritesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public FavouritesController (AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetFavourites()
        {
            var userID = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var favourites = await _context.Favourites.Where(f => f.UserId == userID).ToListAsync();
            return Ok(favourites);
        }

        [HttpPost]
        public async Task<IActionResult> AddFavourites([FromBody] AddFavouriteRequest request)
        {
            var userID = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var favourite = new Favourite
            {
                UserId = userID,
                RestaurantName = request.RestaurantName,
                Address = request.Address,
                Rating = request.Rating,
                Cuisine = request.Cuisine,
                PriceRange = request.PriceRange,
            };
            _context.Favourites.Add(favourite);
            await _context.SaveChangesAsync();
            return Ok(favourite);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFavourite(Guid id)
        {
            var userID = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var favourite = await _context.Favourites.FirstOrDefaultAsync(f=> f.Id == id && f.UserId == userID);
            if (favourite == null) return NotFound("Favourite not found");
            _context.Favourites.Remove(favourite);
            await _context.SaveChangesAsync();
            return Ok("Favourite removed");
        }
    }
}
