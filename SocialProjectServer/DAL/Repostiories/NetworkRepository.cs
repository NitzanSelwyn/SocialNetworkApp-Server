using Amazon.DynamoDBv2.DocumentModel;
using Common.Configs;
using Common.Contracts;
using Common.Contracts.Databases;
using Common.Enums;
using Common.Models.TempModels;
using Common.ResponseModels;
using Newtonsoft.Json;
using SocialProjectServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class NetworkRepository : INetworkRepository
    {
        public INetworkDatabase networkDb { get; set; }
        public NetworkRepository(INetworkDatabase networkDb)
        {
            this.networkDb = networkDb;
            Document user = networkDb.GetUsersTable().GetItem("shahafd");
            user["BlockedList"] = "ffd";
            networkDb.GetUsersTable().PutItem(user);
        }
        public Document GetUserDocById(string id)
        {
            //returns the user doc that matches this id
            return networkDb.GetUsersTable().GetItem(id);
        }
        public User GetUserById(string id)
        {
            //returns the user that matches this id,null if not exists
            Document userDoc = GetUserDocById(id);
            if (userDoc != null)
            {
                return new User(userDoc[DatabaseConfigs.UsersKey], userDoc["FirstName"], userDoc["LastName"], userDoc["Password"], userDoc["Email"], Convert.ToDateTime(userDoc["BirthDate"]), userDoc["Address"], userDoc["WorkLocation"], userDoc["BlockedList"]);
            }
            else
            {
                return null;
            }
        }

        public ResponseEnum BlockUser(string userId, string onUserId)
        {
            //tries to block this user
            try
            {
                User user = GetUserById(userId);
                if (!user.Blocking.Contains(onUserId))
                {
                    user.Blocking.Add(onUserId);
                    Document userDoc = GetUserDocById(userId);
                    userDoc["BlockedList"] = JsonConvert.SerializeObject(user.Blocking);
                }
                return ResponseEnum.Succeeded;

            }
            catch
            {
                return ResponseEnum.Failed;
            }
        }

        public User RegisterUser(UserRegister userRegister)
        {
            //registers the users
            try
            {
                Document newUser = new Document();
                newUser[DatabaseConfigs.UsersKey] = userRegister.Username;
                newUser["FirstName"] = userRegister.FirstName;
                newUser["LastName"] = userRegister.LastName;
                newUser["Password"] = userRegister.Password;
                newUser["BirthDate"] = userRegister.BirthDate;
                newUser["Email"] = userRegister.Email;
                newUser["Address"] = userRegister.Address;
                newUser["WorkLocation"] = userRegister.WorkLocation;
                newUser["BlockedList"] = "";
                networkDb.GetUsersTable().PutItem(newUser);
                return GetUserById(userRegister.Username);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<UserRepresentation> GetBlockedUsers(string userId)
        {
            //returns the username and full name(represntation) of all the users that this user blocked
            List<UserRepresentation> blockedUsers = new List<UserRepresentation>();
            User thisUser = GetUserById(userId);
            foreach (var Id in thisUser.Blocking)
            {
                User blockedUser = GetUserById(Id);
                blockedUsers.Add(new UserRepresentation(blockedUser.ID, blockedUser.ToString()));
            }
            return blockedUsers;
        }
    }
}
