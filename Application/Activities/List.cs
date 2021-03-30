using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

// Application layer to query activities: using MediatR interface IRequest and IRequestHandler
namespace Application.Activities
{
    public class List
    {
        // Query class will return a list of Activities as the generic type of Result class
        public class Query : IRequest<Result<List<Activity>>> { }

        // Pass Query into Handler and return a list of Activities
        public class Handler : IRequestHandler<Query, Result<List<Activity>>>
        {
            // inject DataContext into constructor
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                // context is the database
                _context = context;
            }
            // Handle is an async method
            public async Task<Result<List<Activity>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // eagerly loading related data: explicitly include Attendees and AppUser
                var activities = await _context.Activities
                    .Include(a => a.Attendees)
                    .ThenInclude(u => u.AppUser)
                    .ToListAsync();
                return Result<List<Activity>>.Success(activities);
            }
        }
    }
}