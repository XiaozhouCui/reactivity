using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class Create
    {
        // return CommentDto inside Result of IRequest
        public class Command : IRequest<Result<CommentDto>>
        {
            // comment body and acitvity ID
            public string Body { get; set; }
            public Guid ActivityId { get; set; }
        }
        // add validator to prevent empty comment, using FluentValidation
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Body).NotEmpty();
            }
        }
        // create handler, implement IRequestHandler interface
        public class Handler : IRequestHandler<Command, Result<CommentDto>>
        {
            // inject DataContext (from Persistence) to update db with the new comments
            // inject IMapper (from AutoMapper) to shape the return data
            // inject IUserAccessor from (Application.Interfaces) to get current login user
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _mapper = mapper;
                _context = context;
            }

            // need server to generate id for comments
            public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                // get activity from db
                var activity = await _context.Activities.FindAsync(request.ActivityId);

                if (activity == null) return null;

                // get user from db, pupulate image property using Include() from EntityFrameworkCore
                var user = await _context.Users
                    .Include(p => p.Photos)
                    .SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                // create new comment
                var comment = new Comment
                {
                    Author = user,
                    Activity = activity,
                    Body = request.Body
                };

                activity.Comments.Add(comment); // in memory, no comment ID yet

                // save comment into db
                var success = await _context.SaveChangesAsync() > 0;
                
                // map comment to commentDto and return as result
                if (success) return Result<CommentDto>.Success(_mapper.Map<CommentDto>(comment));

                return Result<CommentDto>.Failure("Failed to add comment");
            }
        }
    }
}