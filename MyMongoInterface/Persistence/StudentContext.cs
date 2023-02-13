using MongoDB.Driver;
using MyMongoInterface.Models.Entities;

namespace MyMongoInterface.Persistence;

public class StudentContext
{
    protected readonly IMongoDatabase database;

    public StudentContext(string database, IConfiguration configuration)
    {
        this.database = new MongoClient(configuration.GetConnectionString("MongoDB")).GetDatabase(database);

        Students = this.database.GetCollection<Student>("students");
        Courses = this.database.GetCollection<Course>("courses");
    }

    public IMongoCollection<Student> Students { get; private set; }

    public IMongoCollection<Course> Courses { get; private set; }
}
