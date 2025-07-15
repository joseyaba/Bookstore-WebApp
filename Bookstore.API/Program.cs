using BookStore.API.Data;
using BookStore.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// Service Configuration
// =======================

// Register MVC controllers
builder.Services.AddControllers();

// Enable CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors();

// Required for Swagger & minimal APIs
builder.Services.AddEndpointsApiExplorer();

// Register AuthService as implementation of IAuthService
builder.Services.AddScoped<IAuthService, AuthService>();

// Register TokenService for generating JWT tokens
builder.Services.AddScoped<TokenService>();

builder.Services.AddScoped<IBookService, BookService>();

// Register EF Core DbContext (configured later with MySQL)
builder.Services.AddDbContext<BookStoreContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Swagger Configuration
// =======================
builder.Services.AddSwaggerGen(options =>
{
    // Configure Swagger to include JWT auth header
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.  
                        Enter 'Bearer' [space] and then your token in the text input below.  
                        Example: Bearer abc123xyz456",
        Name = "Authorization", // Name of the header
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Add the security requirement to all Swagger endpoints
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


// EF Core + MySQL Setup
// =======================

// Configure EF Core to use MySQL with connection string from appsettings.json
builder.Services.AddDbContext<BookStoreContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Ensure TokenService is injected where needed (repeated but safe)
builder.Services.AddScoped<TokenService>();


// JWT Authentication Setup
// =======================

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Token validation settings
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Ensure the token's issuer matches
            ValidateAudience = true, // Ensure the token's audience matches
            ValidateLifetime = true, // Ensure token hasn't expired
            ValidateIssuerSigningKey = true, // Ensure signature is valid

            // These values must match those used when creating the token
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });



// Build the App Pipeline
// =======================
var app = builder.Build();


// Middleware Pipeline
// =======================

// Enable Swagger only in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Force HTTPS redirection
app.UseHttpsRedirection();

// Enable CORS to allow requests from any origin
app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod());

// Apply authentication before checking authorization
app.UseAuthentication();

// Apply authorization logic (based on [Authorize] attributes)
app.UseAuthorization();

// Map endpoints to controller actions
app.MapControllers();

// Start the web application
app.Run();
