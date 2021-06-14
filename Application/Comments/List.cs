using System;
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

namespace Application.Comments
{
    // List is a Query as opposed to a Command
    public class List
    {
        // A List of CommentDto in Result of IRequest
        public class Query : IRequest<Result<List<CommentDto>>>
        {
            // activity associated with this comment list
            public Guid ActivityId { get; set; }
        }

        // List handler, implement IRequestHandler interface, pass in Query as type
        public class Handler : IRequestHandler<Query, Result<List<CommentDto>>>
        {
            // inject DataContext (from Persistence) to update db with the new comments
            // inject IMapper (from AutoMapper) to shape the return data
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<List<CommentDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // get a list of comments from db for a particular activity, order by creation date, map to CommentDto
                var comments = await _context.Comments
                    .Where(x => x.Activity.Id == request.ActivityId) // Linq
                    .OrderBy(x => x.CreatedAt) // Linq
                    .ProjectTo<CommentDto>(_mapper.ConfigurationProvider) // AutoMapper
                    .ToListAsync(); // EntityFrameworkCore

                return Result<List<CommentDto>>.Success(comments);
            }
        }
    }
}