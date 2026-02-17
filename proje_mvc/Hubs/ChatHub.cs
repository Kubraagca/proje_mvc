using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using proje_mvc.Models;
using System;

namespace proje_mvc.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ProjeDbContext _context;

        public ChatHub(ProjeDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string user, string message)
        {
            var msg = new MesajModel
            {
                gonderen_ad = user,
                icerik = message,
                tarih = DateTime.Now,
                oda_adi = "Genel"
            };

            _context.mvc_mesajlar.Add(msg);
            await _context.SaveChangesAsync();

            await Clients.All.SendAsync("ReceiveMessage", user, message, msg.tarih.ToString("HH:mm"));
        }

        public async Task SendPrivateMessage(string sender, string receiver, string message)
        {
            var msg = new MesajModel
            {
                gonderen_ad = sender,
                alici_ad = receiver,
                icerik = message,
                tarih = DateTime.Now,
                oda_adi = $"{sender}-{receiver}"
            };

            _context.mvc_mesajlar.Add(msg);
            await _context.SaveChangesAsync();

            // Hem gönderene hem alıcıya mesajı ilet
            await Clients.All.SendAsync("ReceivePrivateMessage", sender, receiver, message, msg.tarih.ToString("HH:mm"));
        }
    }
}
