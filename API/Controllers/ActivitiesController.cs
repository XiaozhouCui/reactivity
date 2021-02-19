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
        // endpoint to get a list of activities
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
            return await Mediator.Send(new Details.Query { Id = id }); // set Id = id when class is initialised
        }

        // endpoint to create an activity
        [HttpPost]
        // FromBody attribute helps locate the activity obj
        public async Task<IActionResult> CreateActivity([FromBody]Activity activity)
        {
            return Ok(await Mediator.Send(new Create.Command { Activity = activity }));
        }
    }
}