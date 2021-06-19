using System.Threading.Tasks;
using Application.Followers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FollowController : BaseApiController
    {
        // send once will follow, send twice will un-follow (remove record from UserFollowings table)
        [HttpPost("{username}")] // POST .../api/follow/bob
        public async Task<IActionResult> Follow(string username)
        {
            // send a new instance of FollowToggle class from Application layer
            return HandleResult(await Mediator.Send(new FollowToggle.Command { TargetUsername = username }));
        }

        [HttpGet("{username}")] // GET .../api/follow/bob?predicate=followers
        public async Task<IActionResult> GetFollowings(string username, string predicate) // predicate from query string
        {
            return HandleResult(await Mediator.Send(new List.Query { Username = username, Predicate = predicate }));
        }
    }
}