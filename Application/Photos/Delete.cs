using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
        }

        // implement the MediatR interface IRequestHandler will bring Handle() method
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IPhotoAccessor _photoAccessor;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _photoAccessor = photoAccessor;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // get user's object, eagerly load Photos collection
                var user = await _context.Users.Include(p => p.Photos)
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                if (user == null) return null;
                // user already retrieved from db, LINQ FirstOrDefault is OK, no need to us async
                var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);

                if (photo == null) return null;

                // user cannot delete the main photo
                if (photo.IsMain) return Result<Unit>.Failure("You cannot delete your main photo");

                // delete the photo from Cloudinary
                var result = await _photoAccessor.DeletePhoto(photo.Id);

                // if api call failed, return message
                if (result == null) return Result<Unit>.Failure("Problem deleting photo from Cloudinary");

                // remove photo from user's Photos collection
                user.Photos.Remove(photo);

                // save changes to database
                var success = await _context.SaveChangesAsync() > 0;

                if (success) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Problem deleting photo from API");
            }
        }
    }
}