using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace SocioleeMarkingApi.Common
{
	public sealed class ChatHub : Hub
	{
		// A thread-safe dictionary to map connection IDs to usernames
		private static readonly ConcurrentDictionary<string, string> UserConnections = new();

		public async Task SendMessageToChat(Guid chatId, object message)
		{
			var userId = Context.User.FindFirstValue("UserId");
			if (userId != null)
			{
				await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", message, userId);
			}
		}

		public async Task JoinChatGroup(Guid chatId)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
		}

		public async Task TestMethod()
		{
			await Clients.Caller.SendAsync("ReceiveMessage", "Test message");
		}

		public override async Task OnConnectedAsync()
		{
			var userId = Context.User.FindFirstValue("UserId");
			if (userId != null)
			{
				UserConnections.TryAdd(userId, Context.ConnectionId);
				await base.OnConnectedAsync();
			}
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			await base.OnDisconnectedAsync(exception);
		}
	}
}

