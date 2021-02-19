using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Edit
    {
        public class Command : IRequest
        {
            // public property, Acitvity obj from client side
            public Activity Activity { get; set; }
        }

        // implement the interface IRequestHandler
        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            // dependency injection
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                // get the activity by ID from DB
                var activity = await _context.Activities.FindAsync(request.Activity.Id);

                // update the fetched activity
                // activity.Title = request.Activity.Title ?? activity.Title;
                _mapper.Map(request.Activity, activity); // use auto mapper, no need to repeat all properties

                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}