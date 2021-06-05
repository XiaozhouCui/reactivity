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
    // set a photo to be the user's main photo
    public class SetMain
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // get user object with eagerly loaded Photos collection
                var user = await _context.Users.Include(p => p.Photos)
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                if (user == null) return null;

                // use LINQ to get the photo by ID synchronously
                var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);
                // use LINQ to get the current Main photo synchronously
                var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

                // remove IsMain tag from current main photo
                if (currentMain != null) currentMain.IsMain = false;

                // set the reqested photo to main photo
                photo.IsMain = true;

                // update datebase
                var success = await _context.SaveChangesAsync() > 0;

                if (success) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Problem setting main photo");
            }
        }
    }
}