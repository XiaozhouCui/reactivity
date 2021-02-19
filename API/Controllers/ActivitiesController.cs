using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Activities;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ActivitiesController : BaseApiController
    {
        // get a list of activities
        [HttpGet]
        public async Task<ActionResult<List<Activity>>> GetActivities()
        {
            // get response from mediator, initiate an instance of List in Activities
            // Mediator is the protected property in parent class BaseApiController
            return await Mediator.Send(new List.Query());
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