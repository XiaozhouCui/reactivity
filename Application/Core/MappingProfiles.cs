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
        }
    }
}