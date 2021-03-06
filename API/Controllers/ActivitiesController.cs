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
        public async Task<IActionResult> GetActivities()
        {
            // get response from mediator, initiate an instance of List in Activities
            // Mediator is the protected property in parent class BaseApiController
            var result = await Mediator.Send(new List.Query());
            // call HandleResult method from BaseApiController for Error Handling (404, 400 etc.)
            return HandleResult(result);
            // return await _context.Activities.ToListAsync();
        }

        // get single activity by ID: activities/id
        [HttpGet("{id}")]
        public async Task<ActionResult> GetActivity(Guid id)
        {
            var result = await Mediator.Send(new Details.Query { Id = id }); // set Id = id when class is initialised

            // call HandleResult method from BaseApiController for Error Handling (404, 400 etc.)
            return HandleResult(result);
        }

        // endpoint to create an activity
        [HttpPost]
        public async Task<IActionResult> CreateActivity(Activity activity)
        {
            return HandleResult(await Mediator.Send(new Create.Command { Activity = activity }));
        }

        // endpoint to update an activity
        [HttpPut("{id}")]
        public async Task<IActionResult> EditActivity(Guid id, Activity activity)
        {
            activity.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command { Activity = activity }));
        }

        // endpoint to delete an activity
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }
    }
}