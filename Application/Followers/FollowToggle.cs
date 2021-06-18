using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
    public class FollowToggle
    {
        public class Command : IRequest<Result<Unit>>
        {
            // only need TargetUsername, while ObserverUsername is available from token
            public string TargetUsername { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor; // contains method GetUsername()
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
            }

            // Task comes from implementing the IRequestHandler interface
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // get the user who is going to follow other users
                var observer = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                // get the target user to be followed
                var target = await _context.Users.FirstOrDefaultAsync(x => x.UserName == request.TargetUsername);

                if (target == null) return null;

                // check if is following in db, pass in composite Primary Key into FindAsync()
                var following = await _context.UserFollowings.FindAsync(observer.Id, target.Id);
                // if not following, set to following
                if (following == null)
                {
                    following = new UserFollowing
                    {
                        Observer = observer,
                        Target = target
                    };
                    // add following in db context
                    _context.UserFollowings.Add(following);
                }
                else
                {
                    // if already following, remove following from db context
                    _context.UserFollowings.Remove(following);
                }
                // persist to db
                var success = await _context.SaveChangesAsync() > 0;
                // if succeed, return 200
                if (success) return Result<Unit>.Success(Unit.Value);
                // if failes
                return Result<Unit>.Failure("Failed to updaate following");
            }


        }
    }
}