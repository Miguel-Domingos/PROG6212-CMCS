using Microsoft.AspNetCore.SignalR;

namespace PROG6212_CMCS.Server.Hubs
{
    public class NotificationsHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("id")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                // Cada usuário tem seu próprio grupo: user:{id}
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst("id")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user:{userId}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}