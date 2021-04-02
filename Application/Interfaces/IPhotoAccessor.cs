using System.Threading.Tasks;
using Application.Photos;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    // this interface has nothing to do with database, only intaract with Cloudinary
    public interface IPhotoAccessor
    {
        // IFormFile represents the file sent with the HTTP request
        // it comes with file properties like size, name etc.
         Task<PhotoUploadResult> AddPhoto(IFormFile file);
         // delete a photo from Cloudinary
         Task<string> DeletePhoto(string publicId);
    }
}