using System;
using System.Collections.Generic;

namespace Domain
{
    public class Activity
    {
        // following properties will form columns in table "Activity"
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
        public bool IsCancelled { get; set; } // host can cancel this activity
        public ICollection<ActivityAttendee> Attendees { get; set; } = new List<ActivityAttendee>(); // initialise "attendees" property in Activity
    }
}