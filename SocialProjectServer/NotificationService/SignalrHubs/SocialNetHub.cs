using Common.Contracts.Services;
using Common.Models;
using Microsoft.AspNet.SignalR;
using NotificationService.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NotificationService.SignalrHubs
{
    public class SocialNetHub : Hub
    {
        private INotificateService notificateService { get; set; }
        public SocialNetHub()
        {
            notificateService = NotificationContainer.container.GetInstance<INotificateService>();
        }
        public void SignIn(string name)
        {
        
          //  if (!notificateService.userConnections.ContainsKey(Context.ConnectionId))//no connectionid
          //  {
          //      notificateService.userConnections.Add(Context.ConnectionId, name);
          //  }
          //  else notificateService.userConnections[Context.ConnectionId] = name;//update connection

            if (!notificateService.userNames.ContainsKey(name))//has connection by name
            {
                notificateService.userNames.Add(name, Context.ConnectionId); // updates name connection
            }
            else notificateService.userNames[name] = Context.ConnectionId;//update name
        }
        public void CheckForNotificationsOnLogin(string username)
        {
            if (notificateService.ClientHaveNotifications(username))
            {
                SendNotificationToClient(username);
            }
        }
        public void SendNotificationToClient(string username)
        {
            //a user requested his notifications (got pushed from the server that he have new notifications)
            Clients.Client(notificateService.userNames[username]).GotNotificationsFromServer(notificateService.GetNotifsForUser(username));

        }
        public List<Notification> GetMyNotifications(string username)
        {
            //returns the client's notifications
            return notificateService.GetNotifsForUser(username);
        }
        public void SignOut(string name)
        {
          //  notificateService.userConnections.Remove(Context.ConnectionId);
            notificateService.userNames.Remove(name);
        }
    }
}