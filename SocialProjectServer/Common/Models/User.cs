using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialProjectServer.Models
{
    public class User
    {
        public User()
        {
            Blocking = new List<string>();
        }
        public User(string Username, string FirstName, string LastName, string Password, string Email, DateTime BirthDate, string Address, string WorkLocation)
        {
            this.Username = Username;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Password = Password;
            this.Email = Email;
            this.BirthDate = BirthDate;
            this.Address = Address;
            this.WorkLocation = WorkLocation;
        }
        public string ID { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
        public string WorkLocation { get; set; }
        public List<string> Following { get; set; }
        public List<string> Blocking { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}