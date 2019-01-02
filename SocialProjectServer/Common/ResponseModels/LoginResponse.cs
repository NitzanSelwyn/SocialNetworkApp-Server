using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ResponseModels
{
    public class LoginResponse
    {
        public string token { get; set; }
        public ResponseEnum status { get; set; }
        public LoginResponse(string token,ResponseEnum status)
        {
            this.token = token;
            this.status = status;
        }
    }
}
