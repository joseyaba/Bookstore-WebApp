namespace BookStore.API.RequestModels
{
    // Represents the login payload expected from the client
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty; // Username entered by user
        public string Password { get; set; } = string.Empty; // Password entered by user
    }
}
