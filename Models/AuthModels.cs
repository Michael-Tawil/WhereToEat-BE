namespace WhereToEat_BE.Models
{
    public class RegisterRequest
    {
        public string Email {  get; set; }
        public string Password { get; set; }

    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
