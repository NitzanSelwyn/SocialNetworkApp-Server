using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.TempModels
{
    public class UserRegister
    {
        public UserRegister()
        {

        }
        //facebook registration
        public UserRegister(string Username,string FirstName,string LastName)
        {
            this.Username = Username;
            this.FirstName = FirstName;
            this.LastName = LastName;
            Address = "_";
            Email = "_";
            WorkLocation = "_";
            Password = "_";
            BirthDate = DateTime.Now;
        }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string WorkLocation { get; set; }
    }
}
