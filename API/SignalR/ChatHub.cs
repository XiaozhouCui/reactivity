using System;
using System.Threading.Tasks;
using Application.Comments;
using MediatR;
using Microsoft.AspNetCore.SignalR;

// SignalR will be added as a service in ApplicationServiceExtensions

namespace API.SignalR
{
    // Instead of API controllers, we use SignalR Hubs as endpoints for client
    public class ChatHub : Hub
    {
        // inject MediatR just like API controller
        private readonly IMediator _mediator;
        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        // connected clients will be able to invoke the methods inside this SignalR Hub
        public async Task SendComment(Create.Command command)
        {
            // command sent from client will include comment body and activity ID
            var comment = await _mediator.Send(command);

            // after a comment is saved in db, it will have a comment id and shaped into CommentDto
            // use "Clients" object to access those who are connected to the SignalR Hub
            await Clients.Group(command.ActivityId.ToString()) // convert GUID into string
                .SendAsync("ReceiveComment", comment.Value); // name the method "ReceiveComment", send back the comment value (Result<CommentDto>)
        }

        // once clients are connected to this SignalR Hub, we want them to join a group
        public override async Task OnConnectedAsync()
        {
            // get activity id from query string from client
            var httpContext = Context.GetHttpContext();
            var activityId = httpContext.Request.Query["activityId"];
            // "Groups" object from SignalR Hub to add connected clients to a group named by activityId
            await Groups.AddToGroupAsync(Context.ConnectionId, activityId);
            // Send down a list of commentDto from db to all the clients, using Comments Handler
            var result = await _mediator.Send(new List.Query{ActivityId = Guid.Parse(activityId)});
            await Clients.Caller.SendAsync("LoadComments", result.Value);
        }
    }
}