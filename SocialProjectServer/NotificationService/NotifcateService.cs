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
        private static Dictionary<string, List<Notification>> NotifCollec { get; set; }//Notification Collection

        //the key is the user to notify                                                                              
        public NotifcateService()
        {
            NotifCollec = new Dictionary<string, List<Notification>>();
        }
        public bool AddNotification(Notification notif)
        {
            //adds a notification to the collection
            try
            {
                if (NotifCollec[notif.ToId] == null)
                {
                    NotifCollec[notif.ToId] = new List<Notification>();
                }
                NotifCollec[notif.ToId].Add(notif);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
           
        }
        public bool ClearNotifsForUser(string username)
        {
            //clears all the notifcations that were waiting for this user
            try
            {
                NotifCollec[username].Clear();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public List<Notification> GetNotifsForUser(string username)
        {
            //returns all the notifications waiting for this user
            return NotifCollec[username];
        }

    }
}