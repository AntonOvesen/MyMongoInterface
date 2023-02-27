using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MyMongoInterface.Extensions;
using MyMongoInterface.Models.DTOs;
using MyMongoInterface.Models.Entities;
using MyMongoInterface.Persistence;
using System;

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

        [HttpPost]
        public async Task<ActionResult<string>> CreateCourse([FromBody] CourseDTO course, [FromServices] StudentContext context)
        {
            var entity = mapper.Map<Course>(course);

            await context.Courses.InsertOneAsync(entity);

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
            return Ok(await context.Courses.DeleteOneAsync(x => x.Id == id));
        }
    }
}
