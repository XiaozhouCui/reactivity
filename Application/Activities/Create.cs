using System.Threading;
using System.Threading.Tasks;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Create
    {
        // Command do NOT return anything, as opposed to Query
        public class Command : IRequest
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
        public class Handler : IRequestHandler<Command>
        {
            // inject DataContext from Persistence
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                // initialise field "_context" from parameter "context"
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                // currently the _context is in memory, no async
                _context.Activities.Add(request.Activity);

                await _context.SaveChangesAsync();

                return Unit.Value; // equivalent to returning nothing
            }
        }
    }
}