using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.TempModels
{
    public class FacebookUser
    {
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public FacebookUser()
        {

        }
        public FacebookUser(string Username, string Firstname, string Lastname)
        {
            this.Username = Username;
            this.Firstname = Firstname;
            this.Lastname = Lastname;
        }
    }
}
