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
    }
}