using Microsoft.AspNetCore.Mvc;
using BookStore.API.Models;
using BookStore.API.RequestModels;
using BookStore.API.Data;
using BookStore.API.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BookStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly BookStoreContext _context;
        private readonly TokenService _tokenService;
        private readonly IAuthService _authService;

        // Injects DB context, token generator, and auth service
        public AuthController(BookStoreContext context, TokenService tokenService, IAuthService authService)
        {
            _context = context;
            _tokenService = tokenService;
            _authService = authService;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Try to authenticate and get token
            var token = _authService.Login(request.Username, request.Password);
            if (token == null)
                return Unauthorized("Invalid credentials");

            // Return a dictionary to avoid dynamic binding issues in tests
            return Ok(new Dictionary<string, string> { { "token", token } });
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            // Try to register new user
            var result = _authService.Register(request.Username, request.Password);

            if (!result)
                return BadRequest("Username already exists.");

            return Ok("Registration successful");
        }
    }
}
