using System;

namespace Application.Comments
{
    public class CommentDto
    {
        // AutoMapper will create first 3 properties
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Body { get; set; }

        // need to configure the following 3 properties
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Image { get; set; }
    }
}