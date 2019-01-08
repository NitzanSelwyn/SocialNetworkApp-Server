using Amazon.DynamoDBv2.DocumentModel;
using Common.Configs;
using Common.Contracts;
using Common.Contracts.Databases;
using Common.Enums;
using Common.Models.TempModels;
using Common.ResponseModels;
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
        }
        public User GetUserById(string id)
        {
            //returns the user that matches this id,null if not exists
            Document userDoc = networkDb.GetUsersTable().GetItem(id);
            if (userDoc != null)
            {
                return new User(userDoc[DatabaseConfigs.UsersKey], userDoc["FirstName"], userDoc["LastName"], userDoc["Password"], userDoc["Email"], Convert.ToDateTime(userDoc["BirthDate"]), userDoc["Address"], userDoc["WorkLocation"]);
            }
            else
            {
                return null;
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
                networkDb.GetUsersTable().PutItem(newUser);
                return GetUserById(userRegister.Username);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
