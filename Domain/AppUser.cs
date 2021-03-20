using Microsoft.AspNetCore.Identity;

namespace Domain
{
    // IdentityUser is installed from Nuget
    public class AppUser: IdentityUser
    {
        public string DisplayName { get; set; }
        public string Bio { get; set; }
    }
}