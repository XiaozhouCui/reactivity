using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    // Application (use case) layer to get activity details
    public class Details
    {

        public class Query : IRequest<Result<Activity>>
        {
            // need ID parameter in request Query
            public Guid Id { get; set; }
        }
        // add handler here. input: Query, output: Activity
        public class Handler : IRequestHandler<Query, Result<Activity>>
        {
            // inject DataContext from Persistence
            // initialise field "_context" from paramenter "context"
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            // implement interface IRequestHandler to get the following code
            public async Task<Result<Activity>> Handle(Query request, CancellationToken cancellationToken)
            {
                // get ID from request (Query)
                var activity = await _context.Activities.FindAsync(request.Id); // find by activity ID

                return Result<Activity>.Success(activity);
            }
        }
    }
}