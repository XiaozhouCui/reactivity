using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
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
        // use pre-shaped ActivityDto to replace Activity class in the list, to avoid "object cycle"
        public class Query : IRequest<Result<List<ActivityDto>>> { }

        // Pass Query into Handler and return a list of Activities
        public class Handler : IRequestHandler<Query, Result<List<ActivityDto>>>
        {
            // inject DataContext and IMapper into constructor
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                // context is the database
                _context = context;
            }
            // Handle is an async method
            public async Task<Result<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // eagerly loading related data: explicitly include Attendees and AppUser
                var activities = await _context.Activities
                    .Include(a => a.Attendees)
                    .ThenInclude(u => u.AppUser)
                    .ToListAsync();

                // map a List of Activity to a List of ActivityDto, using AutoMapper
                var activitiesToReturn = _mapper.Map<List<ActivityDto>>(activities);

                return Result<List<ActivityDto>>.Success(activitiesToReturn);
            }
        }
    }
}