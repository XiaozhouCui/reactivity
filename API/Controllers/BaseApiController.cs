using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers
{
    // ApiController attribute: API validation with auto HTTP 400 responses
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        // add fields/properties to be used in derived classes
        private IMediator _mediator;
        // protected property Mediator: dependency injection for MediatR?
        // ??= means if _mediator is null, use the right hand side expression
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices
            .GetService<IMediator>();
        
        // method HandleResult will return ActionResult, passing in Result of type <T> as parameter (result from Mediator.Send())
        protected ActionResult HandleResult<T>(Result<T> result)
        {
            // Error handling: 404, 400 etc.
            if (result == null) return NotFound(); // for item not found in db before edit/delete
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }
    }
}