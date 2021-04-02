namespace Application.Photos
{
    public class PhotoUploadResult
    {
        // get properties that come back from cloudinary upload result class
        public string PublicId { get; set; }
        public string Url { get; set; }
    }
}