using System;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Photos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Photos
{
    // constructor: give access to the Cloudinary configuration
    public class PhotoAccessor : IPhotoAccessor
    {
        // Cloudinary SDK: CloudinaryDotNet
        private readonly Cloudinary _cloudinary;
        public PhotoAccessor(IOptions<CloudinarySettings> config)
        {
            // create a new Cloudinary account, order inside Account() is important
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            // create a new Cloudinary object
            _cloudinary = new Cloudinary(account);
        }

        // implement methods from IPhotoAccessor
        public async Task<PhotoUploadResult> AddPhoto(IFormFile file)
        {
            // make sure there is a file to work with
            if (file.Length > 0)
            {
                // "using" means the stream will be disposed of when finished
                await using var stream = file.OpenReadStream();
                // prepare upload parameters
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    // let Cloudinary to transform the file into a square image
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill")
                };
                // upload the file
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                // if upload failed, throw error
                if (uploadResult.Error != null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }

                // return successful result
                return new PhotoUploadResult
                {
                    PublicId = uploadResult.PublicId,
                    // get HTTPS version of url
                    Url = uploadResult.SecureUrl.ToString()
                };
            }

            // if there is no file to upload, return null
            return null;
        }

        public async Task<string> DeletePhoto(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok" ? result.Result : null;
        }
    }
}