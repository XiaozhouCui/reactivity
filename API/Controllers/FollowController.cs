using System.Threading.Tasks;
using Application.Followers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FollowController : BaseApiController
    {
        // send once will follow, send twice will un-follow (remove record from UserFollowings table)
        [HttpPost("{username}")] // POST .../api/follow/tom
        public async Task<IActionResult> Follow(string username)
        {
            // send a new instance of FollowToggle class from Application layer
            return HandleResult(await Mediator.Send(new FollowToggle.Command{TargetUsername = username}));
        }
    }
}