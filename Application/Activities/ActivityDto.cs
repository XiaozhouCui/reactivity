using System;
using System.Collections.Generic;
using Application.Profiles;

namespace Application.Activities
{
    // use DTO to shape the related data

    public class ActivityDto
    {
        // share the properties of Activity entity in Domain
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
        public string HostUsername { get; set; } // identify which attendee is the host of this activity
        public ICollection<Profile> Profiles { get; set; } // include profiles in DTO as Attendee information
    }
}