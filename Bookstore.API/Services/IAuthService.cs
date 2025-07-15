namespace BookStore.API.Services
{
    // Defines the contract for authentication services (login/register)
    public interface IAuthService
    {
        // Attempts to log in and return a JWT token if credentials are valid
        string? Login(string username, string password);

        // Registers a new user with username and password
        bool Register(string username, string password);
    }
}
