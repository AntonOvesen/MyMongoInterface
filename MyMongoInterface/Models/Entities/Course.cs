using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MyMongoInterface.Models.Entities
{
    public class Course
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("length")]
        public TimeSpan? Length { get; set; }
    }
}
