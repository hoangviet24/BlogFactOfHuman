using AutoMapper;
using FactOfHuman.Dto.AuthDto;
using FactOfHuman.Dto.CommentDto;
using FactOfHuman.Dto.Post;
using FactOfHuman.Dto.Reaction;
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
            // Map RegisterDto to User 
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
            // Map PostBlock to PostBlockDto
            CreateMap<PostBlock, PostBlockDto>();
            //Map Post to PostDto
            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Name).ToList()))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Block))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.Author.Name))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Category.Name));
            // Map Commnet to CommentDto
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.PostName, opt => opt.MapFrom(src => src.Post.Title))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.statusComment, opt => opt.MapFrom(src => StatusComment.Visible.ToString()));
            // Map ReactionDto to Reaction
            CreateMap<CreateReacionDto, Reaction>()
                .ForMember(dest => dest.TargetType, opt => opt.MapFrom(src => TargetType.Post.ToString()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => TypeReaction.Like.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            // Map Reaction to ReactionDto
            CreateMap<Reaction, ReactionDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.TargetType, opt => opt.MapFrom(src => src.TargetType.ToString()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
        }
    }
}
