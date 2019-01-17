
using Common.Configs;
using Common.Contracts;
using Common.Enums;
using Common.Models.TempModels;
using Common.ResponseModels;
using DAL.Databases;
using SocialProjectServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Managers
{
    public class UsersManager : IUsersManager
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
        public User EditUserDetails(User user)
        {
            //tries to edit the user details
            return repository.EditUserDetails(user);
        }
        public bool IsUsernameExists(string userName)
        {
            //checks if the username exists
            return repository.GetUserById(userName.ToLower()) != null;
        }
        public User TryRegister(UserRegister userRegister)
        {
            //tries a user registration
            return repository.RegisterUser(userRegister);
        }

        public User FacebookLogin(FacebookUser user)
        {
            //checks if the user exists, if not adds him to the database 
            if (IsUsernameExists(user.Username))
            {
                return GetUserById(user.Username);
            }
            else
            {
                UserRegister userRegister = new UserRegister(user.Username, user.Firstname, user.Lastname);
                return TryRegister(userRegister);
            }
        }

        public List<User> SearchForUsers(string input)
        {
            //searches for users that matches this input
            return repository.SearchForUsers(input);
        }

        public bool BlockedByUser(string myId, string userToViewId)
        {
            using (var graphContext = new Neo4jDB(DatabaseConfigs.neo4jDBConnectionString, DatabaseConfigs.neo4jDBUserName, DatabaseConfigs.neo4jDBPassword))
            {
                return graphContext.GetBlockedUsers(myId).Contains(userToViewId) || graphContext.GetBlockedUsers(userToViewId).Contains(myId);
            }
        }
    }
}
