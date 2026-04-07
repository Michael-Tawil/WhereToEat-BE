using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhereToEat_BE.Data;
using WhereToEat_BE.Models;
using BCrypt.Net;

namespace WhereToEat_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly AppDbContext _context;
        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u=>u.Email == request.Email);
            if (existingUser != null) {
                return BadRequest("Email already registered");
            }
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            return Ok("Login end point working");      
        }

    }
}
