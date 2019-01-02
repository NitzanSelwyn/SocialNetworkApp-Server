using Common.Enums;
using SocialProjectServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ResponseModels
{
    public class LoginRegisterResponse
    {
        public string token { get; set; }
        public User user { get; set; }
        public LoginRegisterResponse(string token, User user)
        {
            this.token = token;
            this.user = user;
        }
    }
}
