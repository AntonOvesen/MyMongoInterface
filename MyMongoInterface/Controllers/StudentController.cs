using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MyMongoInterface.Models.DTOs;
using MyMongoInterface.Models.Entities;
using MyMongoInterface.Persistence;
using System.Linq;

namespace MyMongoInterface.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IServiceProvider provider;
        private readonly IMapper mapper;

        public StudentsController(IServiceProvider provider, IMapper mapper)
        {
            this.provider = provider;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> CreateStudent([FromBody] StudentDTO student, [FromServices] StudentContext context)
        {
            var entity = mapper.Map<Student>(student);

            await context.Students.InsertOneAsync(entity);

            return Ok(entity.Id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDTO>> GetStudent([FromRoute] string id, [FromServices] StudentContext context)
        {
            var entity = await context.Students.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (entity == null) { return NotFound(); }

            return Ok(mapper.Map<StudentDTO>(entity));
        }

        [HttpGet]
        public async Task<ActionResult<List<StudentDTO>>> GetStudents([FromServices] StudentContext context)
        {
            return Ok(await context.Students.Find(x => true).ToListAsync());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateStudent([FromRoute] string id, [FromBody] StudentDTO student, [FromServices] StudentContext context)
        {
            if (await context.Students.Find(x => x.Id == id).FirstOrDefaultAsync() == null)
            {
                return NotFound();
            }

            var entity = mapper.Map<Student>(student);

            entity.Id = id;

            await context.Students.FindOneAndReplaceAsync(s => s.Id == id, entity);

            return Ok();
        }
      
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStudent([FromRoute] string id, [FromServices] StudentContext context)
        {
            if (await context.Students.Find(x => x.Id == id).FirstOrDefaultAsync() == null)
            {
                return NotFound();
            }

            await context.Students.DeleteOneAsync(x => x.Id == id);

            return Ok();
        }

        [HttpGet("{id}/courses")]
        public async Task<ActionResult<List<CourseDTO>>> GetCoursesForStudent([FromRoute] string id, [FromServices] StudentContext context)
        {
            var student = await context.Students.Find(x => x.Id == id).FirstOrDefaultAsync();

            var courses = await context.Courses.Find(c => student.Courses.Contains(c.Id)).ToListAsync();

            return Ok(courses);
        }

        [HttpPut("{id}/courses/{courseId}/join")]
        public async Task<ActionResult> StudentJoinCourse([FromRoute] string id, [FromRoute] string courseId, [FromServices] StudentContext context)
        {
            bool CourseExists = await context.Courses.Find(x => x.Id == courseId).AnyAsync();
            bool StudentExists = await context.Students.Find(x => x.Id == id).AnyAsync();

            if (!CourseExists || !StudentExists) { return NotFound(); }

            var update = Builders<Student>.Update.AddToSet(x => x.Courses, courseId);

            var student = await context.Students.UpdateOneAsync(x => x.Id == id, update);

            return Ok();
        }

        [HttpPut("{id}/courses/{courseId}/leave")]
        public async Task<ActionResult> StudentLeaveCourse([FromRoute] string id, [FromRoute] string courseId, [FromServices] StudentContext context)
        {
            bool CourseExists = await context.Courses.Find(x => x.Id == courseId).AnyAsync();
            bool StudentExists = await context.Students.Find(x => x.Id == id).AnyAsync();
            bool StudentsHasCourse = await context.Students.Find(x => x.Courses.Contains(courseId)).AnyAsync();

            if (!CourseExists || !StudentExists || !StudentsHasCourse) { return BadRequest(); }

            var update = Builders<Student>.Update.Pull(x => x.Courses, courseId);

            var student = await context.Students.UpdateOneAsync(x => x.Id == id, update);

            return Ok();
        }
    }
}
