using Common.Contracts.Services;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotificationService
{
    public class NotifcateService : INotificateService
    {
        public Dictionary<string, List<Notification>> NotifCollec { get; set; }//Notification Collection
        public Dictionary<string, string> userConnections { get; set; }
        public Dictionary<string, string> userNames { get; set; }

        public NotifcateService()
        {
            NotifCollec = new Dictionary<string, List<Notification>>();
            userConnections = new Dictionary<string, string>();
            userNames = new Dictionary<string, string>();
        }
        public void AddNotification(Notification notif,string fullName)
        {
            //adds a notification to the collection

            if (!NotifCollec.Keys.Contains(notif.ToId))
            {
                NotifCollec[notif.ToId] = new List<Notification>();
            }
            notif.FromId = fullName;
            NotifCollec[notif.ToId].Add(notif);

        }

        public bool ClearNotifsForUser(string username)
        {
            //clears all the notifcations that were waiting for this user
            try
            {
                NotifCollec[username].Clear();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public List<Notification> GetNotifsForUser(string username)
        {
            //returns all the notifications waiting for this user
            return NotifCollec[username];
        }
        public bool ClientHaveNotifications(string username)
        {
            //checks if the clients have notifications waiting for him
            try
            {
                return NotifCollec[username].Count > 0;

            }
            catch (Exception e)
            {
                return false;
            }

        }
    }
}