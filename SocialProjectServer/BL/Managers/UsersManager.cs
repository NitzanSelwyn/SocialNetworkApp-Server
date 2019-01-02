using Common.Contracts;
using Common.Models.TempModels;
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

        public User TryLogin(UserLogin userLogin)
        {
            //tries a user login
            User user = repository.GetUserById(userLogin.Username.ToLower());
            if (user != null)
            {
                if (user.Password != userLogin.Password)
                {
                    user = null;
                }
            }
            return user;
        }

        public bool IsUsernameExists(string userName)
        {
            //checks if the username exists
            return repository.GetUserById(userName.ToLower()) != null;
        }
    }
}
