using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class Details
    {
        // use "Query" to deal with database
        public class Query : IRequest<Result<Profile>>
        {
            // users can get other users profiles
            public string Username { get; set; }
        }

        // implement interface of IRequestHandler
        public class Handler : IRequestHandler<Query, Result<Profile>>
        {
            // inject DataContext to access database
            // inject IMapper to map user obj to profile obj
            private readonly DataContext _context; // initialise field from parameter
            private readonly IMapper _mapper; // initialise field from parameter
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            // task will return user profile
            public async Task<Result<Profile>> Handle(Query request, CancellationToken cancellationToken)
            {
                // include user's photos using ProjectTo from AutoMapper.QueryableExtensions to project user to profile
                var user = await _context.Users
                    .ProjectTo<Profile>(_mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync(x => x.Username == request.Username);

                if (user == null) return null;

                return Result<Profile>.Success(user);
            }
        }
    }
}