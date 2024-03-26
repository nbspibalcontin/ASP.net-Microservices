using AutoMapper;
using StudentService.DTOs.Request;
using StudentService.DTOs.Response;
using StudentService.Entity;

namespace Backend
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Student, StudentRequest>();
            CreateMap<StudentRequest, Student>();

            /* CreateMap<Student, StudentDto>()
          .ForMember(dest => dest.ImageData, opt => opt.MapFrom(src => src.Images != null ? Convert.ToBase64String(src.Images.ImageData) : null));
             */

            CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.ImageData, opt => opt.MapFrom(src =>
                src.Images != null && src.Images.ImageData != null ? Convert.ToBase64String(src.Images.ImageData) : null));
        }
    }
}
