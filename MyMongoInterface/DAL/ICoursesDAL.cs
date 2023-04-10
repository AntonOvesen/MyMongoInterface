using AutoMapper;
using MyMongoInterface.Models.DTOs;
using MyMongoInterface.Models.Entities;
using MyMongoInterface.Persistence;

namespace MyMongoInterface.DAL
{
    public interface ICoursesDAL
    {
        public Task<Course> Create(CourseDTO course);
        public Task<Course> GetById(string id);
        public Task<List<Course>> GetAll();
        public Task Update(string id, CourseDTO course);
        public Task Delete(string id);
        public Task<bool> Exists(string id);
    }

    public class CoursesDAL : ICoursesDAL
    {
        private readonly StudentContext context;

        public CoursesDAL(StudentContext context)
        {
            this.context = context;
        }

        public Task<Course> Create(CourseDTO course)
        {
            var entity = mapper.Map<Course>(course);

        }

        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Exists(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Course>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Course> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public Task Update(string id, CourseDTO course)
        {
            throw new NotImplementedException();
        }
    }
}
