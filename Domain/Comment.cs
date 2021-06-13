using System;

namespace Domain
{
    // Comment entity to be used with SignalR
    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; } // comment content
        public AppUser Author { get; set; }
        public Activity Activity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // comments from any timezone are stored in db as UTC time
    }
}