using MyTasks.API.Models;
using MyTasks.API.Models.DTOs;
using MyTasks.Models.Dtos;

namespace MyTasks.API.Services
{
	public interface IAuthService
	{
		Task<User> Register(RegisterDto registerDto);
		Task<string> Login(LoginDto loginDto);
		string GenerateJwtToken(User user);
	}
}
