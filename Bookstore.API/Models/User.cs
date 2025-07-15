namespace BookStore.API.Models
{
    // Represents a registered user in the database
    public class User
    {
        public int Id { get; set; }  // Primary Key (auto-increment)

        public string Username { get; set; } = string.Empty; // Unique username

        public byte[]? PasswordHash { get; set; } // Encrypted password hash

        public byte[]? PasswordSalt { get; set; } // Salt used when hashing password
    }
}
