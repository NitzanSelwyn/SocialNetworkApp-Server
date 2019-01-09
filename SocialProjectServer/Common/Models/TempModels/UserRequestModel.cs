using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.TempModels
{
    public class UserRequestModel
    {
        public string userId { get; set; } //the id of the user that made this request
        public string onUserId { get; set; }// the id of the user that the requester made on (Example: userId wants to block onUserId)
        public string token { get; set; }
        public UserRequestEnum requestType { get; set; }
        public UserRequestModel(string userId, string onUserId, string token, UserRequestEnum requestType)
        {
            this.userId = userId;
            this.onUserId = onUserId;
            this.requestType = requestType;
            this.token = token;
        }
        public UserRequestModel()
        {

        }
    }
}
