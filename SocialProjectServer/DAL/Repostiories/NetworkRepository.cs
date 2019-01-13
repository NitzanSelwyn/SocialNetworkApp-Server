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

        private const string neo4jDBConnectionString = "bolt://ec2-34-245-150-157.eu-west-1.compute.amazonaws.com:7687";
        private const string neo4jDBUserName = "neo4j";
        private const string neo4jDBPassword = "123456";

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
            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
                return graphContext.BlockUser(userId, onUserId);
            }
        }

        public ResponseEnum UnBlockUser(string userId, string onUserId)
        {
            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
                return graphContext.UnBlockUser(userId, onUserId);
            }
        }

        public ResponseEnum FollowUser(string userId, string onUserId)
        {
            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
                return graphContext.FollowUser(userId, onUserId);
            }
        }

        public ResponseEnum UnFollowUser(string userId, string onUserId)
        {
            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
                return graphContext.UnFollowUser(userId, onUserId);
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
        public User EditUserDetails(User user)
        {
            //registers the users
            try
            {
                Document existingUser = GetUserDocById(user.ID);
                existingUser["FirstName"] = user.FirstName;
                existingUser["LastName"] = user.LastName;
                existingUser["BirthDate"] = user.BirthDate;
                existingUser["Email"] = user.Email;
                existingUser["Address"] = user.Address;
                existingUser["WorkLocation"] = user.WorkLocation;
                networkDb.GetUsersTable().PutItem(existingUser);
                return GetUserById(user.Username);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<UserRepresentation> GetBlockedUsers(string userId)
        {
            List<string> userNames = new List<string>();

            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
                userNames = graphContext.GetBlockedUsers(userId);
            }

            return GetUserRepresentations(userNames);

        }

        public List<UserRepresentation> GetFollowingUsers(string userId)
        {
            List<string> userNames = new List<string>();

            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
                userNames = graphContext.GetFollowingUsers(userId);
            }

            return GetUserRepresentations(userNames);
        }

        private List<UserRepresentation> GetUserRepresentations(List<string> userNameList)
        {
            List<UserRepresentation> userRepresentations = new List<UserRepresentation>();

            foreach (var userName in userNameList)
            {
                var user = GetUserById(userName);
                userRepresentations.Add(new UserRepresentation(user.Username, $"{user.FirstName} {user.LastName}"));
            }

            return userRepresentations;
        }
    }
}
