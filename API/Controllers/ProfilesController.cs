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
            return HandleResult(await Mediator.Send(new Details.Query{Username = username}));
        }
    }
}