using Common.Contracts;
using SocialProjectServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Managers
{
    public class UsersManager:IUsersManager
    {
        public INetworkRepository repository { get; set; }
        public UsersManager(INetworkRepository repository)
        {
            this.repository = repository;
        }
        public User GetUserById(string id)
        {
            //returns the user that matches this id
            return repository.GetUserById(id);
        }
    }
}
