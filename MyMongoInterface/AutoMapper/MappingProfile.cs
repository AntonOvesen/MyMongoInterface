using AutoMapper;
using MyMongoInterface.Models.DTOs;
using MyMongoInterface.Models.Entities;

namespace MyMongoInterface.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<StudentDTO, Student>().ReverseMap();
        CreateMap<CourseDTO, Course>().ForMember(x => x.Length, dest => dest.MapFrom(src => src.Days.HasValue ? new TimeSpan(src.Days.Value, 0, 0, 0) : default));
        CreateMap<Course, CourseDTO>().ForMember(x => x.Days, dest => dest.MapFrom(src => src.Length.Value.Days));
    }
}
