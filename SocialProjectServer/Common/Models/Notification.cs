using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Notification
    {
        public string FromId { get; set; }
        public string ToId { get; set; }
        public DateTime DateOfNotify { get; set; } //the date of the event
        NotificationEnum NotifyType { get; set; }
        public Notification()
        {

        }
        public Notification(string FromId,string ToId,DateTime DateOfNotify,NotificationEnum NotifyType)
        {
            this.FromId = FromId;
            this.ToId = ToId;
            this.DateOfNotify = DateOfNotify;
            this.NotifyType = NotifyType;
        }
    }
}
