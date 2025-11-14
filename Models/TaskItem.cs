using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyTasks.API.Models
{
	public class TaskItem
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		[BsonElement("title")]
		public string Title { get; set; }

		[BsonElement("description")]
		public string Description { get; set; }

		[BsonElement("isCompleted")]
		public bool IsCompleted { get; set; }

		[BsonElement("createdAt")]
		public DateTime CreatedAt { get; set; }

		[BsonElement("updatedAt")]
		public DateTime UpdatedAt { get; set; }

		[BsonElement("userId")]
		[BsonRepresentation(BsonType.ObjectId)]
		public string UserId { get; set; }
	}
}
