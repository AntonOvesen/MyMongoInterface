using MyMongoInterface.Models.DTOs;
using MyMongoInterface.Models.Entities;

namespace MyMongoInterface.Extensions
{
    public static class ModelExtensions
    {
        public static Course UpdateFromDTO(this Course course, CourseDTO dto)
        {
            if (dto.Days.HasValue) { course.Length = new TimeSpan(dto.Days.Value, 0, 0, 0); }

            if (!string.IsNullOrEmpty(dto.Title)) { course.Title = dto.Title!; }

            return course;
        }
    }
}
