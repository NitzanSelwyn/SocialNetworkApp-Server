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
        public string FromFullName{ get; set; }
        public string ToId { get; set; }
        public string PostId { get; set; }
        public DateTime DateOfNotify { get; set; } //the date of the event
        NotificationEnum NotifyType { get; set; }
        public Notification()
        {

        }
        public Notification(string FromId,string ToId,string FromFullName,DateTime DateOfNotify,NotificationEnum NotifyType,string PostId=null)
        {
            this.FromId = FromId;
            this.FromFullName = FromFullName;
            this.ToId = ToId;
            this.DateOfNotify = DateOfNotify;
            this.NotifyType = NotifyType;
            this.PostId = PostId;
        }
    }
}
