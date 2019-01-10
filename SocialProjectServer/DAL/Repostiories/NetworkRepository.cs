using Amazon.DynamoDBv2.DocumentModel;
using Common.Configs;
using Common.Contracts;
using Common.Enums;
using Common.Models.TempModels;
using DAL.Databases;
using Newtonsoft.Json;
using SocialProjectServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repostiories
{
    public class NetworkRepository : INetworkRepository
    {
        public INetworkDatabase networkDb { get; set; }
        public NetworkRepository(INetworkDatabase networkDb)
        {
            this.networkDb = networkDb;

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
                return new User(userDoc[DatabaseConfigs.UsersKey], userDoc["FirstName"], userDoc["LastName"], userDoc["Password"], userDoc["Email"], Convert.ToDateTime(userDoc["BirthDate"]), userDoc["Address"], userDoc["WorkLocation"]);
            }
            else
            {
                return null;
            }
        }

        public ResponseEnum BlockUser(string userId, string onUserId)
        {
            //tries to block this user
            //Neo4j implementation
            return ResponseEnum.Failed;
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
           //neo4j implementation
            return blockedUsers;
        }
    }
}
