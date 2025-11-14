using MyTasks.API.Models;
using MyTasks.Models.Dtos;

namespace MyTasks.API.Services
{
	public interface ITaskService
	{
		Task<List<TaskItem>> GetUserTasks(string userId);
		Task<TaskItem> GetTaskById(string id, string userId);
		Task<TaskItem> CreateTask(CreateTaskDto createTaskDto, string userId);
		Task<TaskItem> UpdateTask(string id, UpdateTaskDto updateTaskDto, string userId);
		Task<bool> DeleteTask(string id, string userId);
	}
}
