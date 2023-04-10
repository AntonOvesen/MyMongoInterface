using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MyMongoInterface.DAL;
using MyMongoInterface.Extensions;
using MyMongoInterface.Models.DTOs;
using MyMongoInterface.Models.Entities;
using MyMongoInterface.Persistence;
using System;
using System.Linq;

namespace MyMongoInterface.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly IServiceProvider provider;
        private readonly IMapper mapper;
        private readonly ICoursesDAL coursesDAL;

        public CoursesController(IServiceProvider provider, IMapper mapper, ICoursesDAL coursesDAL)
        {
            this.provider = provider;
            this.mapper = mapper;
            this.coursesDAL = coursesDAL;
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateCourse([FromBody] CourseDTO course)
        {
            var entity = await coursesDAL.Create(course);
            

            return Ok(entity.Id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDTO>> GetCourse([FromRoute] string id, [FromServices] StudentContext context)
        {
            var entity = await context.Courses.AsQueryable().FirstOrDefaultAsync(x => x.Id == id);

            return entity != null ? Ok(entity) : NotFound(id);
        }
        
        [HttpGet]
        public async Task<ActionResult<List<CourseDTO>>> GetCourses([FromServices] StudentContext context)
        {
            return Ok(await context.Courses.AsQueryable().ToListAsync());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCourse([FromRoute] string id, [FromBody] CourseDTO course, [FromServices] StudentContext context)
        {
            var entity = await context.Courses.AsQueryable().FirstOrDefaultAsync(x => x.Id == id);
            
            entity.Id = id;

            entity.UpdateFromDTO(course);

            await context.Courses.ReplaceOneAsync(x => x.Id == id, entity);
            
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCourse([FromRoute] string id, [FromServices] StudentContext context)
        {
            // Transactions like this doesnt work on "Standalone server". Aka hosting one in docker not cool enough.
            //await context.ExecuteTransactionAsync(async (s, ct) =>
            //{
            //    await context.Courses.DeleteOneAsync(x => x.Id == id);

            //    var pullUpdate = Builders<Student>.Update.Pull(x => x.Courses, id);

            //    return context.Students.UpdateManyAsync(x => x.Courses.Contains(id), pullUpdate);
            //});

            await context.Courses.DeleteOneAsync(x => x.Id == id);

            var pullUpdate = Builders<Student>.Update.Pull(x => x.Courses, id);

            await context.Students.UpdateManyAsync(x => x.Courses.Contains(id), pullUpdate);

            return Ok();
        }
    }
}
