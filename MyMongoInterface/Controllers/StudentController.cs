using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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
            var entity = await context.Students.AsQueryable().FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) { return NotFound(); }

            return Ok(mapper.Map<StudentDTO>(entity));
        }

        [HttpGet]
        public async Task<ActionResult<List<StudentDTO>>> GetStudents([FromServices] StudentContext context)
        {
            return Ok(await context.Students.AsQueryable().ToListAsync());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateStudent([FromRoute] string id, [FromBody] StudentDTO student, [FromServices] StudentContext context)
        {
            
            if (await context.Students.AsQueryable().AnyAsync(x => x.Id == id))
            {
                return NotFound();
            }

            var entity = mapper.Map<Student>(student);

            entity.Id = id;

            await context.Students.ReplaceOneAsync(s => s.Id == id, entity);

            return Ok();
        }
      
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStudent([FromRoute] string id, [FromServices] StudentContext context)
        {
            if (await context.Students.AsQueryable().AnyAsync(x => x.Id == id))
            {
                return NotFound();
            }

            await context.Students.DeleteOneAsync(x => x.Id == id);

            return Ok();
        }

        [HttpGet("{id}/courses")]
        public async Task<ActionResult<List<CourseDTO>>> GetCoursesForStudent([FromRoute] string id, [FromServices] StudentContext context)
        {
            var student = await context.Students.AsQueryable().FirstOrDefaultAsync(x => x.Id == id);

            var courses = await context.Courses.AsQueryable().Where(c => student.Courses.Contains(c.Id)).ToListAsync();

            return Ok(courses);
        }

        [HttpPut("{id}/courses/{courseId}/join")]
        public async Task<ActionResult> StudentJoinCourse([FromRoute] string id, [FromRoute] string courseId, [FromServices] StudentContext context)
        {
            bool CourseExists = await context.Courses.AsQueryable().AnyAsync(x => x.Id == courseId);
            bool StudentExists = await context.Students.AsQueryable().AnyAsync(x => x.Id == id);

            if (!CourseExists || !StudentExists) { return NotFound(); }

            var student = await context.Students.AsQueryable().FirstAsync(x => x.Id == id);
            
            student.Courses.Add(courseId);

            await context.Students.ReplaceOneAsync(x => x.Id == id, student);

            return Ok();
        }

        [HttpPut("{id}/courses/{courseId}/leave")]
        public async Task<ActionResult> StudentLeaveCourse([FromRoute] string id, [FromRoute] string courseId, [FromServices] StudentContext context)
        {
            var student = await context.Students.AsQueryable().FirstOrDefaultAsync(x => x.Id == id);

            if (student == null || !student.Courses.Contains(courseId)) { return BadRequest(); }

            student.Courses.Remove(courseId);

            await context.Students.ReplaceOneAsync(x => x.Id == id, student);

            return Ok();
        }
    }
}
