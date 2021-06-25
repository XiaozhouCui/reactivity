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
            // pass currentUsername as ProjectTo() parameter to configuration in List?
            string currentUsername = null;
            // where we map from, and where to map to
            CreateMap<Activity, Activity>();
            // map Activity to ActivityDto, and configure properties
            CreateMap<Activity, ActivityDto>()
                .ForMember(dest => dest.HostUsername, options => options.MapFrom(source => source.Attendees
                    .FirstOrDefault(x => x.IsHost).AppUser.UserName));
            // map ActivityAttendee to AttendeeDto
            CreateMap<ActivityAttendee, AttendeeDto>()
                .ForMember(dest => dest.DisplayName, options => options.MapFrom(source => source.AppUser.DisplayName))
                .ForMember(dest => dest.Username, options => options.MapFrom(source => source.AppUser.UserName))
                .ForMember(dest => dest.Bio, options => options.MapFrom(source => source.AppUser.Bio))
                .ForMember(d => d.Image, o => o.MapFrom(s => s.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.FollowersCount, o => o.MapFrom(s => s.AppUser.Followers.Count))
                .ForMember(d => d.FollowingCount, o => o.MapFrom(s => s.AppUser.Followings.Count))
                .ForMember(d => d.Following, o => o.MapFrom(s => s.AppUser.Followers.Any(x => x.Observer.UserName == currentUsername)));
            // map AppUser to Profile (self-defined Profile, not AutoMapper Profile) to update image, and following
            CreateMap<AppUser, Profiles.Profile>()
                .ForMember(d => d.Image, o => o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.FollowersCount, o => o.MapFrom(s => s.Followers.Count))
                .ForMember(d => d.FollowingCount, o => o.MapFrom(s => s.Followings.Count))
                .ForMember(d => d.Following, o => o.MapFrom(s => s.Followers.Any(x => x.Observer.UserName == currentUsername)));
            // map from comment to commentDTO
            CreateMap<Comment, CommentDto>()
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.Author.DisplayName))
                .ForMember(d => d.Username, o => o.MapFrom(s => s.Author.UserName))
                .ForMember(d => d.Image, o => o.MapFrom(s => s.Author.Photos.FirstOrDefault(x => x.IsMain).Url));
            // map ActivityAttendee to UserActivityDto
            CreateMap<ActivityAttendee, Profiles.UserActivityDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Activity.Id)) // populate all properties in UserActivityDto
                .ForMember(d => d.Date, o => o.MapFrom(s => s.Activity.Date))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Activity.Title))
                .ForMember(d => d.Category, o => o.MapFrom(s => s.Activity.Category))
                .ForMember(d => d.HostUsername, o => o.MapFrom(s => s.Activity.Attendees.FirstOrDefault(x => x.IsHost).AppUser.UserName));
        }
    }
}