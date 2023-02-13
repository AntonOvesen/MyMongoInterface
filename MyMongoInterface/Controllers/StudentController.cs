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

        private StudentContext Context => provider.CreateScope().ServiceProvider.GetRequiredService<StudentContext>();

        [HttpPost]
        public async Task<ActionResult> CreateStudent([FromBody] StudentDTO student)
        {
            var entity = mapper.Map<Student>(student);

            await Context.Students.InsertOneAsync(entity);
            
            return Ok(entity.Id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDTO>> GetStudent([FromRoute] string id)
        {
            var entity = await Context.Students.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (entity == null) { return NotFound(); }

            return Ok(mapper.Map<StudentDTO>(entity));
        }

        [HttpGet]
        public async Task<ActionResult<List<StudentDTO>>> GetStudents()
        {
            return Ok(await Context.Students.Find(x => true).ToListAsync());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateStudent([FromRoute] string id, [FromBody] StudentDTO student)
        {
            var context = Context;

            if (await context.Students.Find(x => x.Id == id).FirstOrDefaultAsync() == null)
            {
                return NotFound();
            }

            var entity = mapper.Map<Student>(student);

            entity.Id = id;

            await context.Students.FindOneAndReplaceAsync(s => s.Id == id, entity);

            return Ok();
        }

        [HttpPut("{id}/joincourse/{courseId}")]
        public async Task<ActionResult> StudentJoinCourse([FromRoute] string id, [FromRoute] string courseId)
        {
            var context = Context;

            bool CourseExists = await context.Courses.Find(x => x.Id == courseId).AnyAsync();
            bool StudentExists = await context.Students.Find(x => x.Id == id).AnyAsync();
            
            if(!CourseExists || !StudentExists) { return NotFound(); }

            var update = Builders<Student>.Update.AddToSet(x => x.Courses, courseId);
            var student = await context.Students.UpdateOneAsync(x => x.Id == id, update);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStudent([FromRoute] string id)
        {
            var context = Context;

            if (await context.Students.Find(x => x.Id == id).FirstOrDefaultAsync() == null)
            {
                return NotFound();
            }

            await context.Students.DeleteOneAsync(x => x.Id == id);

            return Ok();
        }
    }
}
