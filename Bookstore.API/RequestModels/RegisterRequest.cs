namespace BookStore.API.RequestModels
{
    // Represents the registration payload expected from the client
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty; // New username
        public string Password { get; set; } = string.Empty; // New password
    }
}
