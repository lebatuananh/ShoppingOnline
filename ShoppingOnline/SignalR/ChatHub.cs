using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ShoppingOnline.Application.Systems.Announcements.Dtos;

namespace ShoppingOnline.WebApplication.SignalR
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(AnnouncementViewModel message)
        {
            await Clients.All.SendAsync("ReciveMessage", message);
        }
    }
}