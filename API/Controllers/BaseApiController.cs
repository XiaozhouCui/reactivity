using API.Extensions;
using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers
{
    // ApiController attribute: API validation with auto HTTP 400 responses
    [ApiController]
    [Route("api/[controller]")] // [controller] is a placeholder, can be acitvities, profiles, photos etc.
    public class BaseApiController : ControllerBase
    {
        // add fields/properties to be used in derived classes
        private IMediator _mediator;
        // protected property Mediator: injected in one place and used in all derived controllers, will have Mediator.Send() method
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

        // paginated result
        protected ActionResult HandlePagedResult<T>(Result<PagedList<T>> result)
        {
            // Error handling: 404, 400 etc.
            if (result == null) return NotFound(); // for item not found in db before edit/delete
            if (result.IsSuccess && result.Value != null)
            {
                // add "Pagination" header using HttpExtensions.cs
                Response.AddPaginationHeader(result.Value.CurrentPage, result.Value.PageSize, result.Value.TotalCount, result.Value.TotalPages);
                return Ok(result.Value);
            }
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }
    }
}