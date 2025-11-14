using MongoDB.Driver;
using MyTasks.API.Data;
using MyTasks.API.Models;
using MyTasks.Models.Dtos;

namespace MyTasks.API.Services
{
	public class TaskService : ITaskService
	{
		private readonly MongoDbContext _context;

		public TaskService(MongoDbContext context)
		{
			_context = context;
		}

		public async Task<List<TaskItem>> GetUserTasks(string userId)
		{
			return await _context.Tasks
				.Find(t => t.UserId == userId)
				.SortByDescending(t => t.CreatedAt)
				.ToListAsync();
		}

		public async Task<TaskItem> GetTaskById(string id, string userId)
		{
			return await _context.Tasks
				.Find(t => t.Id == id && t.UserId == userId)
				.FirstOrDefaultAsync();
		}

		public async Task<TaskItem> CreateTask(CreateTaskDto createTaskDto, string userId)
		{
			var task = new TaskItem
			{
				Title = createTaskDto.Title,
				Description = createTaskDto.Description,
				IsCompleted = false,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow,
				UserId = userId
			};

			await _context.Tasks.InsertOneAsync(task);
			return task;
		}

		public async Task<TaskItem> UpdateTask(string id, UpdateTaskDto updateTaskDto, string userId)
		{
			var filter = Builders<TaskItem>.Filter.Eq(t => t.Id, id) &
						 Builders<TaskItem>.Filter.Eq(t => t.UserId, userId);

			var update = Builders<TaskItem>.Update
				.Set(t => t.Title, updateTaskDto.Title)
				.Set(t => t.Description, updateTaskDto.Description)
				.Set(t => t.IsCompleted, updateTaskDto.IsCompleted)
				.Set(t => t.UpdatedAt, DateTime.UtcNow);

			var options = new FindOneAndUpdateOptions<TaskItem>
			{
				ReturnDocument = ReturnDocument.After
			};

			return await _context.Tasks.FindOneAndUpdateAsync(filter, update, options);
		}

		public async Task<bool> DeleteTask(string id, string userId)
		{
			var result = await _context.Tasks
				.DeleteOneAsync(t => t.Id == id && t.UserId == userId);

			return result.DeletedCount > 0;
		}
	}
}
