using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MyTasks.API.Data;
using MyTasks.API.Models.DTOs;
using MyTasks.API.Services;
using MyTasks.Models.Dtos;

namespace MyTasks.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly MongoDbContext _context;

		public AuthController(IAuthService authService, MongoDbContext context)
		{
			_authService = authService;
			_context = context;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterDto registerDto)
		{
			try
			{
				var user = await _authService.Register(registerDto);
				var token = _authService.GenerateJwtToken(user);

				return Ok(new { token, user = new { user.Id, user.Username, user.Email } });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		
		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginDto loginDto)
		{
			try
			{
				// Få token
				var token = await _authService.Login(loginDto);

				// Hämta user från databasen
				var user = await _context.Users
					.Find(u => u.Username == loginDto.Username)
					.FirstOrDefaultAsync();

				// Returnera både token och user
				return Ok(new
				{
					token,
					user = new
					{
						id = user.Id,
						username = user.Username,
						email = user.Email
					}
				});
			}
			catch (Exception ex)
			{
				return Unauthorized(new { message = ex.Message });
			}
		}

	}
}
