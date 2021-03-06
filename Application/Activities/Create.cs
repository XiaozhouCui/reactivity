using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Create
    {
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
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                // initialise field "_context" from parameter "context"
                _context = context;
            }

            // Result of type Mediator Unit
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // _context is the database object
                _context.Activities.Add(request.Activity);

                // check result of saving changes to database
                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to create activity");

                return Result<Unit>.Success(Unit.Value); // Unit.Value is equivalent to returning nothing, only notifying success
            }
        }
    }
}