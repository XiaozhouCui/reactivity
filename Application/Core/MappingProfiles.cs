using System.Linq;
using Application.Activities;
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
                .ForMember(dest => dest.HostUsername, opt => opt.MapFrom(src => src.Attendees
                    .FirstOrDefault(x => x.IsHost).AppUser.UserName));
            // map ActivityAttendee to Profile (self-defined Profile, not AutoMapper Profile)
            CreateMap<ActivityAttendee, Profiles.Profile>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.AppUser.DisplayName))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.AppUser.UserName))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.AppUser.Bio));
        }
    }
}