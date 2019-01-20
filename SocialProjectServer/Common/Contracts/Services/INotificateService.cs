using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Contracts.Services
{
    public interface INotificateService
    {
        Dictionary<string, List<Notification>> NotifCollec { get; set; }
        Dictionary<string, string> userConnections { get; set; }
        Dictionary<string, string> userNames { get; set; }
        void AddNotification(Notification notif,string fullName);
        bool ClearNotifsForUser(string username);
        List<Notification> GetNotifsForUser(string username);
        bool ClientHaveNotifications(string username);
    }
}
