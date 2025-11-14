using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MyTasks.API.Data;
using MyTasks.API.Models;
using MyTasks.API.Models.DTOs;
using MyTasks.Models.Dtos;
using System.Security.Claims;
using System.Text;

namespace MyTasks.API.Services
{
	public class AuthService : IAuthService
	{
		private readonly MongoDbContext _context;
		private readonly IConfiguration _configuration;

		public AuthService(MongoDbContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		public async Task<User> Register(RegisterDto registerDto)
		{
			// Check if user already exists
			var existingUser = await _context.Users
				.Find(u => u.Username == registerDto.Username || u.Email == registerDto.Email)
				.FirstOrDefaultAsync();

			if (existingUser != null)
			{
				throw new Exception("User already exists");
			}

			// Create new user
			var user = new User
			{
				Username = registerDto.Username,
				Email = registerDto.Email,
				PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
				CreatedAt = DateTime.UtcNow
			};

			await _context.Users.InsertOneAsync(user);
			return user;
		}

		public async Task<string> Login(LoginDto loginDto)
		{
			var user = await _context.Users
				.Find(u => u.Username == loginDto.Username)
				.FirstOrDefaultAsync();

			if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
			{
				throw new Exception("Invalid credentials");
			}

			return GenerateJwtToken(user);
		}

		public string GenerateJwtToken(User user)
		{
			var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
			new Claim(ClaimTypes.NameIdentifier, user.Id), // Use ClaimTypes.NameIdentifier
            new Claim(ClaimTypes.Name, user.Username),
			new Claim("userId", user.Id) // Add custom claim for easier access
        }),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(key),
					SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
