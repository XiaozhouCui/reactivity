namespace Domain
{
    public class Photo
    {
        public string Id { get; set; } // Id will be the publicId coming back from Cloudinary
        public string Url { get; set; }
        public bool IsMain { get; set; } // check if it is the user's main image
    }
}