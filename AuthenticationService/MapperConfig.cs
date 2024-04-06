using AuthenticationService.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<RegisterDto, IdentityUser>()
             .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
             .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
             .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));

            CreateMap<LoginDto, IdentityUser>()
             .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
             .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
             .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
        }
    }
}
