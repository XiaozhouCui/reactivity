using System.Linq;
using Application.Activities;
using Application.Comments;
using AutoMapper;
using Domain;

namespace Application.Core
{
    // derive from Profile class (NuGet AutoMapper DI)
    public class MappingProfiles : Profile
    {
        // auto mapper will be added as a service in Startup.cs
        public MappingProfiles()
        {
            // where we map from, and where to map to
            CreateMap<Activity, Activity>();
            // map Activity to ActivityDto, and configure properties
            CreateMap<Activity, ActivityDto>()
                .ForMember(dest => dest.HostUsername, options => options.MapFrom(src => src.Attendees
                    .FirstOrDefault(x => x.IsHost).AppUser.UserName));
            // map ActivityAttendee to AttendeeDto
            CreateMap<ActivityAttendee, AttendeeDto>()
                .ForMember(dest => dest.DisplayName, options => options.MapFrom(src => src.AppUser.DisplayName))
                .ForMember(dest => dest.Username, options => options.MapFrom(src => src.AppUser.UserName))
                .ForMember(dest => dest.Bio, options => options.MapFrom(src => src.AppUser.Bio))
                .ForMember(dest => dest.Image, options => options.MapFrom(source => source.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url));
            // map AppUser to Profile (self-defined Profile, not AutoMapper Profile) to update image
            CreateMap<AppUser, Profiles.Profile>()
                .ForMember(dest => dest.Image, options => options.MapFrom(source => source.Photos.FirstOrDefault(x => x.IsMain).Url));
            // map from comment to commentDTO
            CreateMap<Comment, CommentDto>()                
                .ForMember(dest => dest.DisplayName, options => options.MapFrom(src => src.Author.DisplayName))
                .ForMember(dest => dest.Username, options => options.MapFrom(src => src.Author.UserName))
                .ForMember(dest => dest.Image, options => options.MapFrom(source => source.Author.Photos.FirstOrDefault(x => x.IsMain).Url));

        }
    }
}