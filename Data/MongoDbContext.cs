using MongoDB.Driver;
using MyTasks.API.Models;

namespace MyTasks.API.Data
{
	public class MongoDbContext
	{
		private readonly IMongoDatabase _database;

		public MongoDbContext(IConfiguration configuration)
		{
			var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
			_database = client.GetDatabase("MyTasksDB");
		}

		public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
		public IMongoCollection<TaskItem> Tasks => _database.GetCollection<TaskItem>("Tasks");
	}
}
