using AutoMapper;
using FactOfHuman.Dto.AuthDto;
using FactOfHuman.Dto.CommentDto;
using FactOfHuman.Dto.Post;
using FactOfHuman.Dto.Tag;
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
                .ForMember(dest => dest.AuthProvider, opt => opt.MapFrom(src => AuthProvider.Local.ToString()))
                .ForMember(dest=> dest.CreatedAt, opt => opt.MapFrom(src=>DateTime.UtcNow))
                .ForMember(dest=> dest.isActive, opt => opt.MapFrom(src => false))
                .ForMember(dest=> dest.AvatarUrl, opt => opt.MapFrom(src => string.Empty))
                .ForMember(dest=> dest.activeToken, opt => opt.MapFrom(src => string.Empty))
                .ForMember(dest=> dest.Bio, opt => opt.MapFrom(src => string.Empty));
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Role.ToString()));
            CreateMap<PostBlock, PostBlockDto>();
            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Name).ToList()))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Block));
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.PostName, opt => opt.MapFrom(src => src.Post.Title))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.FactName, opt => opt.MapFrom(src => src.Fact.Title))
                .ForMember(dest => dest.statusComment, opt => opt.MapFrom(src => StatusComment.Visible.ToString()));
        }
    }
}
