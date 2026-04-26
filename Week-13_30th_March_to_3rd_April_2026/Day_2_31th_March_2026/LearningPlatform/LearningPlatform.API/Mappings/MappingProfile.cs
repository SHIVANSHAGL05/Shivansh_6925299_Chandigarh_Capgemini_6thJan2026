using AutoMapper;
using LearningPlatform.API.DTOs;
using LearningPlatform.API.Models;

// Alias the domain model to avoid clash with AutoMapper.Profile
using DomainProfile = LearningPlatform.API.Models.Profile;

namespace LearningPlatform.API.Mappings;

public class MappingProfile : AutoMapper.Profile
{
    public MappingProfile()
    {
        // Course mappings
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => src.Instructor.Username))
            .ForMember(dest => dest.LessonCount, opt => opt.MapFrom(src => src.Lessons.Count))
            .ForMember(dest => dest.EnrollmentCount, opt => opt.MapFrom(src => src.Enrollments.Count));

        CreateMap<CreateCourseDto, Course>();
        CreateMap<UpdateCourseDto, Course>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Lesson mappings
        CreateMap<Lesson, LessonDto>();
        CreateMap<CreateLessonDto, Lesson>();

        // Enrollment mappings
        CreateMap<Enrollment, EnrollmentDto>()
            .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title));

        // User mappings
        CreateMap<User, UserDto>();
    }
}
