using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class ActivitiesController : BaseApiController
    {
        // Initialise field from parameter
        private readonly IMediator _mediator;
        // Inject Mediator into constructor
        public ActivitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // get a list of activities
        [HttpGet]
        public async Task<ActionResult<List<Activity>>> GetActivities()
        {
            // get response from mediator, initiate an instance of List in Activities
            return await _mediator.Send(new List.Query());
            // return await _context.Activities.ToListAsync();
        }

        // get single activity by ID: activities/id
        [HttpGet("{id}")] 
        public async Task<ActionResult<Activity>> GetActivity(Guid id)
        {
            return Ok();
        }
    }
}