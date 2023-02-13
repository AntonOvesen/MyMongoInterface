using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MyMongoInterface.Models.DTOs;
using MyMongoInterface.Models.Entities;
using MyMongoInterface.Persistence;

namespace MyMongoInterface.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly IServiceProvider provider;
        private readonly IMapper mapper;

        public CoursesController(IServiceProvider provider, IMapper mapper)
        {
            this.provider = provider;
            this.mapper = mapper;
        }

        private StudentContext Context => provider.CreateScope().ServiceProvider.GetRequiredService<StudentContext>();

        [HttpPost]
        public async Task<ActionResult<string>> CreateCourse([FromBody] CourseDTO course)
        {
            var entity = mapper.Map<Course>(course);

            await Context.Courses.InsertOneAsync(entity);

            return Ok(entity.Id);
        }

        [HttpGet("fromstudent/{id}")]
        public async Task<ActionResult<List<CourseDTO>>> GetCoursesForStudent([FromRoute] string id)
        {
            var context = Context;

            var student = await context.Students.Find(x => x.Id == id).FirstOrDefaultAsync();

            var courses = await context.Courses.Find(c => student.Courses.Contains(c.Id)).ToListAsync();

            return mapper.Map<List<CourseDTO>>(courses);
        }
    }
}
