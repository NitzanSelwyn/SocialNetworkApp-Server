﻿using Amazon.DynamoDBv2.DocumentModel;
using Common.Configs;
using Common.Contracts;
using Common.Contracts.Managers;
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
        public Dictionary<string, User> Users { get; set; }
        private Neo4jDB neo4JDB;
        private static readonly object NetDbLock = new object();

        public NetworkRepository(INetworkDatabase networkDb)
        {
            this.networkDb = networkDb;
            neo4JDB = new Neo4jDB();
            InitAndLoadUsers();
        }

        private void InitAndLoadUsers()
        {
            lock (NetDbLock)
            {
                //inits and loads the users from the database
                Users = new Dictionary<string, User>();
                ScanFilter scanFilter = new ScanFilter();

                ScanOperationConfig config = new ScanOperationConfig()
                {
                    AttributesToGet = new List<string> { DatabaseConfigs.UsersKey },
                    Filter = scanFilter
                };
                config.Select = SelectValues.SpecificAttributes;
                Search search = networkDb.GetUsersTable().Scan(config);
                if (search.Count > 0)
                {
                    foreach (var item in search.GetNextSet())
                    {
                        Users.Add(item[DatabaseConfigs.UsersKey], GetUserById(item[DatabaseConfigs.UsersKey]));
                    }
                }
            }
        }

        public Document GetUserDocById(string id)
        {
            lock (NetDbLock)
            {
                //returns the user doc that matches this id
                try
                {
                    Document userDoc = networkDb.GetUsersTable().GetItem(id);
                    return userDoc;
                }
                catch(Exception)
                {
                    return null;
                }
            }
        }

        public User GetUserFromDoc(Document userDoc)
        {
            lock (NetDbLock)
            {
                //retreives a user from it's doc
                return new User(userDoc[DatabaseConfigs.UsersKey], userDoc["FirstName"], userDoc["LastName"], userDoc["Password"], userDoc["Email"], Convert.ToDateTime(userDoc["BirthDate"]), userDoc["Address"], userDoc["WorkLocation"]);
            }
        }

        public User GetUserById(string id)
        {
            lock (NetDbLock)
            {
                //returns the user that matches this id,null if not exists
                Document userDoc = GetUserDocById(id);
                if (userDoc != null)
                {
                    return GetUserFromDoc(userDoc);
                }
                else
                {
                    return null;
                }
            }
        }

        public ResponseEnum BlockUser(string userId, string onUserId)
        {
            lock (NetDbLock)
            {
                return neo4JDB.BlockUser(userId, onUserId);
            }
        }

        public ResponseEnum UnBlockUser(string userId, string onUserId)
        {
            lock (NetDbLock)
            {
                return neo4JDB.UnBlockUser(userId, onUserId);
            }
        }

        public ResponseEnum FollowUser(string userId, string onUserId)
        {
            lock (NetDbLock)
            {
                return neo4JDB.FollowUser(userId, onUserId);
            }
        }

        public ResponseEnum UnFollowUser(string userId, string onUserId)
        {
            lock (NetDbLock)
            {
                return neo4JDB.UnFollowUser(userId, onUserId);
            }
        }

        public User RegisterUser(UserRegister userRegister)
        {
            //registers the users
            try
            {
                Document newUser = new Document();
                newUser[DatabaseConfigs.UsersKey] = userRegister.Username.ToLower();
                newUser["FirstName"] = userRegister.FirstName.ToLower();
                newUser["LastName"] = userRegister.LastName.ToLower();
                newUser["Password"] = userRegister.Password;
                newUser["BirthDate"] = userRegister.BirthDate;
                newUser["Email"] = userRegister.Email;
                newUser["Address"] = userRegister.Address.ToLower();
                newUser["WorkLocation"] = userRegister.WorkLocation.ToLower();
                networkDb.GetUsersTable().PutItem(newUser);
                User user = GetUserById(userRegister.Username);
                Users.Add(user.Username, user);

                neo4JDB.RegisterUserToNeo4j(userRegister.Username, userRegister.FirstName, userRegister.LastName);

                return user;
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
                Document existingUser = GetUserDocById(user.Username);
                existingUser["FirstName"] = user.FirstName.ToLower();
                existingUser["LastName"] = user.LastName.ToLower();
                existingUser["BirthDate"] = user.BirthDate;
                existingUser["Email"] = user.Email;
                existingUser["Address"] = user.Address.ToLower();
                existingUser["WorkLocation"] = user.WorkLocation.ToLower();
                networkDb.GetUsersTable().PutItem(existingUser);
                User userEdited = GetUserById(user.Username);
                Users[userEdited.Username] = userEdited;

                neo4JDB.UpdateUserDetails(userEdited.Username, userEdited.FirstName, userEdited.LastName);

                return userEdited;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<UserRepresentation> GetBlockedUsers(string userId)
        {
            lock (NetDbLock)
            {
                List<string> userNames = new List<string>();

                userNames = neo4JDB.GetBlockedUsers(userId);

                return GetUserRepresentations(userNames);
            }
        }

        public List<UserRepresentation> GetFollowingUsers(string userId)
        {
            List<string> userNames = new List<string>();

            userNames = neo4JDB.GetTheUsersThatIFollow(userId);

            return GetUserRepresentations(userNames);
        }

        public List<UserRepresentation> GetUsersThatFollowMe(string userId)
        {
            List<string> userNames = new List<string>();

            userNames = neo4JDB.GetTheUserThatFollowMe(userId);


            return GetUserRepresentations(userNames);
        }

        public List<UserRepresentation> GetUsersThatFollowsMe(string userId)
        {
            lock (NetDbLock)
            {
                List<string> userNames = new List<string>();

                userNames = neo4JDB.GetTheUserThatFollowMe(userId);

                return GetUserRepresentations(userNames);
            }
        }

        private List<UserRepresentation> GetUserRepresentations(List<string> userNameList)
        {
            lock (NetDbLock)
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

        public ResponseEnum EditPassword(EditPassword editPassword)
        {
            lock (NetDbLock)
            {
                try
                {
                    Document userDoc = GetUserDocById(editPassword.Username);
                    userDoc["Password"] = editPassword.NewPassword;
                    networkDb.GetUsersTable().PutItem(userDoc);
                    return ResponseEnum.Succeeded;
                }
                catch (Exception)
                {
                    return ResponseEnum.Failed;
                }
            }
        }

        public List<User> SearchForUsers(string input)
        {
            lock (NetDbLock)
            {
                input = input.ToLower();
                return Users.Values.Where(u => u.Username.ToLower().Contains(input) || u.FirstName.ToLower().Contains(input) || u.LastName.ToLower().Contains(input)).ToList();
            }
        }

        public string GetLastPostIdAndUpdate()
        {
            lock (NetDbLock)
            {
                //gets  the last post id and updates the prop
                var table = networkDb.GetConfigsTable();
                Document idDoc = table.GetItem("LastPostId");
                var lastId = idDoc["Value"];
                int id = Convert.ToInt32(lastId);
                id++;
                idDoc["Value"] = id.ToString();
                table.PutItem(idDoc);
                return id.ToString();
            }
        }
    }
}
