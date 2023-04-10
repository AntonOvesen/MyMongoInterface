using AutoMapper;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MyMongoInterface.Models.DTOs;
using MyMongoInterface.Models.Entities;
using MyMongoInterface.Persistence;

namespace MyMongoInterface.DAL
{
    public interface IStudentDAL
    {
        public Task<Student> Create(StudentDTO student);
        public Task<Student> GetById(string id);
        public Task<List<Student>> GetAll();
        public Task Update(string id, StudentDTO student);
        public Task Delete(string id);
        public Task<bool> Exists(string id);
    }

    public class StudentDAL : IStudentDAL
    {
        private readonly StudentContext context;
        private readonly IMapper mapper;

        public StudentDAL(StudentContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<Student> Create(StudentDTO student)
        {
            var entity = mapper.Map<Student>(student);

            await context.Students.InsertOneAsync(entity);

            return entity;
        }

        public Task<Student> GetById(string id)
        {
            return context.Students.AsQueryable().FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<Student>> GetAll()
        {
            return context.Students.AsQueryable().ToListAsync();
        }

        public Task Update(string id, StudentDTO student)
        {
            var entity = mapper.Map<Student>(student);
            entity.Id = id;
            return context.Students.ReplaceOneAsync(s => s.Id == id, entity);
        }

        public Task Delete(string id)
        {
            return context.Students.DeleteOneAsync(x => x.Id == id);
        }

        public Task<bool> Exists(string id)
        {
            return context.Students.AsQueryable().AnyAsync(x => x.Id == id);
        }
    }
}
