using AutoMapper;
using FactOfHuman.Dto.AuthDto;
using FactOfHuman.Dto.UserDto;
using FactOfHuman.Enum;
using FactOfHuman.Models;

namespace FactOfHuman.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Role.Reader.ToString()))
                .ForMember(dest=> dest.CreatedAt, opt => opt.MapFrom(src=>DateTime.UtcNow))
                .ForMember(dest=> dest.isActive, opt => opt.MapFrom(src => false))
                .ForMember(dest=> dest.AvatarUrl, opt => opt.MapFrom(src => string.Empty))
                .ForMember(dest=> dest.activeToken, opt => opt.MapFrom(src => string.Empty))
                .ForMember(dest=> dest.Bio, opt => opt.MapFrom(src => string.Empty));
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Role.ToString()));
        }
    }
}
