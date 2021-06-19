using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
    public class List
    {
        public class Query : IRequest<Result<List<Profiles.Profile>>>
        {
            public string Predicate { get; set; } // what to return: followers or followees
            public string Username { get; set; }
        }

        // req: Query, res: list of Profile (disamb: not AutoMapper.Profile)
        public class Handler : IRequestHandler<Query, Result<List<Profiles.Profile>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<List<Profiles.Profile>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // declare profiles
                var profiles = new List<Profiles.Profile>();
                // check if predicate is followers or followees
                switch (request.Predicate)
                {
                    case "followers": // a list of observers in UserFollowings table whose target username is {xxx}
                        profiles = await _context.UserFollowings
                            .Where(x => x.Target.UserName == request.Username) // Linq
                            .Select(u => u.Observer) // select only observers, not targets
                            .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider) // AutoMapper
                            .ToListAsync(); // Entity Framework
                        break;
                    case "following": // a list of targets in UserFollowings table whose observer username is {xxx}
                        profiles = await _context.UserFollowings
                            .Where(x => x.Observer.UserName == request.Username) // Linq
                            .Select(u => u.Target) // select only observers, not targets
                            .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider) // AutoMapper
                            .ToListAsync(); // Entity Framework
                        break;
                }
                // return a list of profiles
                return Result<List<Profiles.Profile>>.Success(profiles);
            }
        }
    }
}