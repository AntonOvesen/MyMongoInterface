using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyMongoInterface.Models.Entities;

[BsonIgnoreExtraElements]
public class Student
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("graduated")]
    public bool Graduated { get; set; }

    [BsonElement("courses"), BsonRepresentation(BsonType.ObjectId)]
    public List<string> Courses { get; set; } = new List<string>();

    [BsonElement("gender")]
    public string Gender { get; set; } = string.Empty;

    [BsonElement("age")]
    public int Age { get; set; }
}
