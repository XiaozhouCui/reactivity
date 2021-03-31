using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    // Application (use case) layer to get activity details
    public class Details
    {

        public class Query : IRequest<Result<ActivityDto>>
        {
            // need ID parameter in request Query
            public Guid Id { get; set; }
        }
        // add handler here. input: Query, output: Activity
        public class Handler : IRequestHandler<Query, Result<ActivityDto>>
        {
            // inject DataContext from Persistence, and IMapper from AutoMapper
            // initialise field "_context" from paramenter "context"
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            // implement interface IRequestHandler to get the following code
            public async Task<Result<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                // get ID from request (Query), and find by activity ID
                var activity = await _context.Activities
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                    // .FindAsync(request.Id); // FindAsync does NOT work with Projection

                return Result<ActivityDto>.Success(activity);
            }
        }
    }
}