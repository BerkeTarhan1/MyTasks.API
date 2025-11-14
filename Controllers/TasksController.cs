using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTasks.API.Models;
using MyTasks.API.Services;
using MyTasks.Models.Dtos;
using System.Security.Claims;

namespace MyTasks.API.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class TasksController : ControllerBase
	{
		private readonly ITaskService _taskService;
		private readonly ILogger<TasksController> _logger;

		public TasksController(ITaskService taskService, ILogger<TasksController> logger)
		{
			_taskService = taskService;
			_logger = logger;
		}

		private string GetUserId()
		{
			// Try multiple claim types to find the user ID
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
					  ?? User.FindFirst("userId")?.Value
					  ?? User.FindFirst("id")?.Value;

			_logger.LogInformation($"User ID from token: {userId}");
			_logger.LogInformation($"All claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}: {c.Value}"))}");

			if (string.IsNullOrEmpty(userId))
			{
				throw new Exception("User ID not found in token");
			}

			return userId;
		}

		[HttpGet]
		public async Task<IActionResult> GetUserTasks()
		{
			try
			{
				var userId = GetUserId();
				var tasks = await _taskService.GetUserTasks(userId);
				return Ok(tasks);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting user tasks");
				return Unauthorized(new { message = "Invalid token" });
			}
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetTask(string id)
		{
			try
			{
				var userId = GetUserId();
				var task = await _taskService.GetTaskById(id, userId);

				if (task == null)
					return NotFound();

				return Ok(task);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting task");
				return Unauthorized(new { message = "Invalid token" });
			}
		}

		[HttpPost]
		public async Task<IActionResult> CreateTask(CreateTaskDto createTaskDto)
		{
			try
			{
				var userId = GetUserId();
				var task = await _taskService.CreateTask(createTaskDto, userId);
				return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating task");
				return Unauthorized(new { message = "Invalid token" });
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateTask(string id, UpdateTaskDto updateTaskDto)
		{
			try
			{
				var userId = GetUserId();
				var task = await _taskService.UpdateTask(id, updateTaskDto, userId);

				if (task == null)
					return NotFound();

				return Ok(task);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating task");
				return Unauthorized(new { message = "Invalid token" });
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTask(string id)
		{
			try
			{
				var userId = GetUserId();
				var result = await _taskService.DeleteTask(id, userId);

				if (!result)
					return NotFound();

				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting task");
				return Unauthorized(new { message = "Invalid token" });
			}
		}
	}
}
