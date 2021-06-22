using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        public class Query : IRequest<Result<PagedList<ActivityDto>>>
        {
            public PagingParams Params { get; set; }
        }

        // req: Query, res: a (paginated) list of ActivityDTO
        public class Handler : IRequestHandler<Query, Result<PagedList<ActivityDto>>>
        {
            // inject DataContext, IMapper and IUserAccessor into constructor
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _mapper = mapper;
                // context is the database
                _context = context;
            }
            // Handle is an async method
            public async Task<Result<PagedList<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                /*========== EAGERLY LOADING RELATED DATA ==========*/

                // // Eagerly loading related data: explicitly include Attendees and AppUser
                // var activities = await _context.Activities
                //     .Include(a => a.Attendees)
                //     .ThenInclude(u => u.AppUser)
                //     .ToListAsync();

                // // map a List of Activity to a List of ActivityDto, using AutoMapper
                // var activitiesToReturn = _mapper.Map<List<ActivityDto>>(activities);

                // return Result<List<ActivityDto>>.Success(activitiesToReturn);

                /*========== USE PROJECTION TO LOAD RELATED DATA ==========*/

                // Projection comes from AutoMapper QueryableExtensions, it makes SQL query much cleaner
                var query = _context.Activities
                    .OrderBy(d => d.Date)
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider,
                        new { currentUsername = _userAccessor.GetUsername() })
                    // .ToListAsync(cancellationToken);
                    .AsQueryable(); // defer the query execution, not async/await anymore

                return Result<PagedList<ActivityDto>>.Success(
                    // directly call the static method CreateAsync() from PagedList class
                    await PagedList<ActivityDto>.CreateAsync(query, request.Params.PageNumber, request.Params.PageSize)
                );
            }
        }
    }
}