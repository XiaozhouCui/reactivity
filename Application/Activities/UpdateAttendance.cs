using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    // this handler has 3 purposes:
    // - join an activity
    // - remove an attendee from activity
    // - host cancel the activity
    public class UpdateAttendance
    {
        // Command class implements IRequest interface from MediatR
        // Command do NOT return anything, only the result type of Mediator Unit (Unit.Value means nothing)
        public class Command : IRequest<Result<Unit>>
        {
            // Id comes from client side request
            public Guid Id { get; set; }
        }

        // Handler class implements IRequestHandler interface from MediatR
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            // need to know the activity and the user making this request: inject DataContext and IUserAccessor
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // get the activity and its related data with eager loading (using EntityFrameworkCore)
                var activity = await _context.Activities
                    .Include(a => a.Attendees).ThenInclude(u => u.AppUser)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                if (activity == null) return null; // 404 not found

                // get the user making this request
                var user = await _context.Users.FirstAsync(x => x.UserName == _userAccessor.GetUsername());

                if (user == null) return null; // 404 not found

                // not async method: activity already loaded from db
                // defensive approach: optional chaining
                var hostUsername = activity.Attendees.FirstOrDefault(x => x.IsHost)?.AppUser?.UserName;
                
                var attendance = activity.Attendees.FirstOrDefault(x => x.AppUser.UserName == user.UserName);

                // if the host is making this request, then toggle the IsCancelled status
                if (attendance != null && hostUsername == user.UserName)
                    activity.IsCancelled = !activity.IsCancelled;

                // if an attendee is making this request, remove the attendee from activity
                if (attendance != null && hostUsername != user.UserName)
                    activity.Attendees.Remove(attendance);
                
                // if a non-attendee user is making this request, add that user to the attendee list
                if (attendance == null)
                {
                    attendance = new ActivityAttendee
                    {
                        AppUser = user,
                        Activity = activity,
                        IsHost = false
                    };

                    activity.Attendees.Add(attendance);
                }

                // persist the changes to database
                var result = await _context.SaveChangesAsync() > 0;

                // return the result (Unit.Value means nothing)
                return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem updating attendance");
            }
        }
    }
}