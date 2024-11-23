﻿using Microsoft.AspNetCore.SignalR;
namespace testZugether1.Hubs
{
	public class ChatHub : Hub
	{
		public async Task SendMessage(string user, string message, string searchMemberIdToAvatar, string newBasement)
		{
			try
			{
				Console.WriteLine($"接收到訊息: {user} - {message}");
				await Clients.All.SendAsync("NewMessage", user, message, searchMemberIdToAvatar, newBasement);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"發送消息失敗: {ex.Message}");
			}
		}
	}
}
