using System.Threading.Tasks;
using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // handle the returning profile from Application.Profiles
    public class ProfilesController : BaseApiController
    {
        [HttpGet("{username}")]

        public async Task<IActionResult> GetProfile(string username)
        {
            return HandleResult(await Mediator.Send(new Details.Query { Username = username }));
        }

        // add an endpoint for editing user profile
        [HttpPut]
        public async Task<IActionResult> Edit(Edit.Command command)
        {
            return HandleResult(await Mediator.Send(command));
        }

        // get activities by username and predicate (future, past, hosting)
        [HttpGet("{username}/activities")] // /api/profiles/bob/activities?predicate=past
        public async Task<IActionResult> GetUserActivities(string username, string predicate)
        {
            // username from url params, predicate from query string
            return HandleResult(await Mediator.Send(new ListActivities.Query
            { Username = username, Predicate = predicate }));
        }
    }
}