namespace MyTasks.API.Models.DTOs
{
	public class LoginDto
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}
}

// Models/Dtos/RegisterDto.cs
namespace MyTasks.Models.Dtos
{
	public class RegisterDto
	{
		public string Username { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
	}
}

// Models/Dtos/CreateTaskDto.cs
namespace MyTasks.Models.Dtos
{
	public class CreateTaskDto
	{
		public string Title { get; set; }
		public string Description { get; set; }
	}
}

// Models/Dtos/UpdateTaskDto.cs
namespace MyTasks.Models.Dtos
{
	public class UpdateTaskDto
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public bool IsCompleted { get; set; }
	}
}

