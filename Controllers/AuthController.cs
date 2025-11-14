using Microsoft.AspNetCore.Mvc;
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

		public AuthController(IAuthService authService)
		{
			_authService = authService;
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
				var token = await _authService.Login(loginDto);
				return Ok(new { token });
			}
			catch (Exception ex)
			{
				return Unauthorized(new { message = ex.Message });
			}
		}
	}
}
