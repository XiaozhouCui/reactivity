namespace Domain
{
    // self-referencing many-to-many relationship: a join table
    public class UserFollowing
    {
        public string ObserverId { get; set; }
        public AppUser Observer { get; set; } // a person who is going to follow another user
        public string TargetId { get; set; }
        public AppUser Target { get; set; } // the target user to follow
    }
}