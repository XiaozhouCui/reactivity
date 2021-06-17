using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
    // IdentityUser is installed from Nuget
    public class AppUser: IdentityUser
    {
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public ICollection<ActivityAttendee> Activities { get; set; } // Many-to-Many or One-to-Many ?
        public ICollection<Photo> Photos { get; set; } // One-to-Many relation
        public ICollection<UserFollowing> Followings { get; set; } // targets I am following
        public ICollection<UserFollowing> Followers { get; set; } // users who are following me
    }
}