using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class Create
    {
        // Command class implements IRequest interface from MediatR
        // Command do NOT return anything, only the result type of Mediator Unit (Unit.Value means nothing)
        public class Command : IRequest<Result<Unit>>
        {
            // public property, Acitvity obj from client side
            public Activity Activity { get; set; }
        }

        // validate the activity data in POST request
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                // Command class contains Activity, use ActivityValidator in Application layer as rules
                RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
            }
        }

        // need to implement IRequestHandler interface to include Handle() method
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            // inject DataContext from Persistence
            // inject IUserAccessor from Application
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                // initialise field "_context" from parameter "context"
                _context = context;
            }

            // Result of type Mediator Unit
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // _context is the database object
                // get access to user object from db
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                // create a new attendee who host/start the activity
                var attendee = new ActivityAttendee
                {
                    AppUser = user,
                    Activity = request.Activity,
                    IsHost = true
                };

                // add the host to the attendee of Activity
                request.Activity.Attendees.Add(attendee);
                
                _context.Activities.Add(request.Activity);

                // check result of saving changes to database
                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to create activity");

                return Result<Unit>.Success(Unit.Value); // Unit.Value is equivalent to returning nothing, only notifying success
            }
        }
    }
}