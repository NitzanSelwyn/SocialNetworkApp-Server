using Common.Contracts.Services;
using Common.Enums;
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
        public Dictionary<string, string> userNames { get; set; }

        public NotifcateService()
        {
            NotifCollec = new Dictionary<string, List<Notification>>();
            userNames = new Dictionary<string, string>();

            AddNotification(new Notification("shahaf", "shahafd", "shahaf", DateTime.Now, NotificationEnum.Followed));
            AddNotification(new Notification("shahaf", "shahafd", "shahaf", DateTime.Now, NotificationEnum.Commented_On_Post, "342522"));
            AddNotification(new Notification("shahaf", "shahafd", "shahaf", DateTime.Now, NotificationEnum.Liked_Post, "321312"));
            AddNotification(new Notification("shahaf", "shahafd", "shahaf", DateTime.Now, NotificationEnum.Tagged_In_Post, "3666"));

        }
        public void AddNotification(Notification notif)
        {
            //adds a notification to the collection
            if (!NotifCollec.Keys.Contains(notif.ToId))
            {
                NotifCollec[notif.ToId] = new List<Notification>();
            }
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
            try
            {
                return NotifCollec[username];
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public bool ClientHaveNotifications(string username)
        {
            //checks if the clients have notifications waiting for him
            try
            {
                return NotifCollec[username].Count > 0;

            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}