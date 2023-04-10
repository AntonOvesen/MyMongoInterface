using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MyMongoInterface.DAL;
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
        private readonly IMapper mapper;
        private readonly IStudentDAL studentDAL;

        public StudentsController(IMapper mapper, IStudentDAL studentDAL)
        {
            this.mapper = mapper;
            this.studentDAL = studentDAL;
        }

        [HttpPost]
        public async Task<ActionResult> CreateStudent([FromBody] StudentDTO student)
        {
            var entity = await studentDAL.Create(student);

            return Ok(entity.Id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDTO>> GetStudent([FromRoute] string id)
        {
            var entity = await studentDAL.GetById(id);

            if (entity == null) { return NotFound(); }

            return Ok(mapper.Map<StudentDTO>(entity));
        }

        [HttpGet]
        public async Task<ActionResult<List<StudentDTO>>> GetStudents()
        {
            return Ok(await studentDAL.GetAll());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateStudent([FromRoute] string id, [FromBody] StudentDTO student)
        {
            await studentDAL.Update(id, student);

            return Ok();
        }
      
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStudent([FromRoute] string id)
        {
            if (!await studentDAL.Exists(id))
            {
                return NotFound();
            }

            await studentDAL.Delete(id);

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
