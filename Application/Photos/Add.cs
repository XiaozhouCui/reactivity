using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Add
    {
        // API returns the publicId and Url of photo
        public class Command : IRequest<Result<Photo>>
        {
            // the form-data in POST request must be called "File" as below
            public IFormFile File { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Photo>>
        {
            // need db context to save photo url into database
            private readonly DataContext _context;
            private readonly IPhotoAccessor _photoAccessor;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _photoAccessor = photoAccessor;
                _context = context;
            }

            public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
            {
                // get user from database using EAGER LOADING
                var user = await _context.Users.Include(p => p.Photos)
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                if (user == null) return null;

                // add photo to Cloudinary
                // no try/catch needed, if failed, _photoAccessor will throw an exception
                var photoUploadResult = await _photoAccessor.AddPhoto(request.File);

                var photo = new Photo
                {
                    Url = photoUploadResult.Url,
                    Id = photoUploadResult.PublicId
                };

                // check if user already has any photo set to "mail"
                if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;

                // add photo to user's Photos collection
                user.Photos.Add(photo);

                // save changes to database
                var result = await _context.SaveChangesAsync() > 0;

                // check if result is successful
                if (result) return Result<Photo>.Success(photo);

                return Result<Photo>.Failure("Problem adding photo");
            }
        }
    }
}