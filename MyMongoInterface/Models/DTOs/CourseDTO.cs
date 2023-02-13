using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MyMongoInterface.Models.DTOs
{
    public class CourseDTO
    {
        public string Title { get; set; }

        public int Days { get; set; }
    }
}
