using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

// Logic to query activities: using MediatR interface IRequest and IRequestHandler
namespace Application.Activities
{
    public class List
    {
        // Query class will return a list of Activities
        public class Query : IRequest<List<Activity>> { }

        // Pass Query into Handler and return a list of Activities
        public class Handler : IRequestHandler<Query, List<Activity>>
        {
            // inject DataContext into constructor
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            // Handle is an async method
            public async Task<List<Activity>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Activities.ToListAsync();
            }
        }
    }
}