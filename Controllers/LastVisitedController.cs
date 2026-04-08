using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WhereToEat_BE.Data;
using WhereToEat_BE.Models;

namespace WhereToEat_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LastVisitedController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LastVisitedController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetLastVisited()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var lastVisited = await _context.LastVisited
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.VisitedAt)
                .Take(20)
                .ToListAsync();

            return Ok(lastVisited);
        }

        [HttpPost]
        public async Task<IActionResult> AddLastVisited([FromBody] AddFavouriteRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var lastVisited = new LastVisited
            {
                UserId = userId,
                RestaurantName = request.RestaurantName,
                Address = request.Address,
                Rating = request.Rating,
                Cuisine = request.Cuisine,
                PriceRange = request.PriceRange
            };

            _context.LastVisited.Add(lastVisited);
            await _context.SaveChangesAsync();

            return Ok(lastVisited);
        }
    }
}