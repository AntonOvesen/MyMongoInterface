using AutoMapper;
using MyMongoInterface.Models.DTOs;
using MyMongoInterface.Models.Entities;

namespace MyMongoInterface.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<StudentDTO, Student>().ReverseMap();
        CreateMap<CourseDTO, Course>().ForMember(x => x.Length, dest => dest.MapFrom(src => new TimeSpan(src.Days, 0, 0, 0)));
        CreateMap<Course, CourseDTO>().ForMember(x => x.Days, dest => dest.MapFrom(src => src.Length.Days));
    }
}
