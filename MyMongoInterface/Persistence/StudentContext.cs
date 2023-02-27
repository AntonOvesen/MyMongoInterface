using MongoDB.Driver;
using MyMongoInterface.Abstractions;
using MyMongoInterface.Models.Entities;

namespace MyMongoInterface.Persistence;

public class StudentContext : MongoDBContext
{
    public StudentContext(string database, IConfiguration configuration) : base(database, configuration)
    {
        Students = this.database.GetCollection<Student>("students");
        Courses = this.database.GetCollection<Course>("courses");       
    }

    public IMongoCollection<Student> Students { get; private set; } 

    public IMongoCollection<Course> Courses { get; private set; }
}