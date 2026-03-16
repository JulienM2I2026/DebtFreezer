using Microsoft.AspNetCore.SignalR;

namespace Gateway.Hubs
{
    /// <summary>
    /// Hub SignalR pour les notifications en temps réel.
    /// Les clients rejoignent un groupe par userId pour recevoir leurs notifications.
    /// </summary>
    public class NotificationHub : Hub
    {
        // Appelé côté client : connection.invoke("JoinUserGroup", userId)
        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }

        // Appelé côté client : connection.invoke("LeaveUserGroup", userId)
        public async Task LeaveUserGroup(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
        }
    }
}
