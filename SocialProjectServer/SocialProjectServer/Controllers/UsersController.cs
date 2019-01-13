using Common.Configs;
using Common.Contracts;
using Common.Models.TempModels;
using Common.ResponseModels;
using DAL.Databases;
using SocialProjectServer.Containers;
using SocialProjectServer.Models;
using SocialProjectServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SocialProjectServer.Controllers
{
    public class UsersController : ApiController
    {
        IUsersManager usersManager { get; set; }
        IHttpClient httpClient { get; set; }

        private const string neo4jDBConnectionString = "bolt://ec2-34-245-150-157.eu-west-1.compute.amazonaws.com:7687";
        private const string neo4jDBUserName = "neo4j";
        private const string neo4jDBPassword = "123456";

        public UsersController()
        {
            usersManager = ServerContainer.container.GetInstance<IUsersManager>();
            httpClient = ServerContainer.container.GetInstance<HttpClientSender>();
        }

        [HttpPost]
        [Route(RouteConfigs.UserLoginRoute)]
        public IHttpActionResult UserLogin([FromBody]UserLogin userLogin)
        {
            //user's login
            User user = usersManager.TryLogin(userLogin);
            if (user != null)
            {
                string token = GetToken(user.Username);
                return Ok(new LoginRegisterResponse(token, user));
            }
            else
            {
                return Conflict();
            }
        }
        [HttpPost]
        [Route(RouteConfigs.EditUserDetailsRoute)]
        public IHttpActionResult EditUserDetails([FromBody]User userToEdit)
        {
            //edits the user details
            User user = usersManager.EditUserDetails(userToEdit);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return Conflict();
            }
        }
        [HttpPost]
        [Route(RouteConfigs.UserRegisterRoute)]
        public IHttpActionResult UserRegister([FromBody]UserRegister userRegister)
        {
            //tries a new user's registration
            User user = usersManager.TryRegister(userRegister);
            string token = "";
            if (user != null)
            {
                token = GetToken(user.Username);
                using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
                {
                    graphContext.RegisterUserToNeo4j(userRegister.Username);
                }
            }
            return Ok(new LoginRegisterResponse(token, user));
        }
        [HttpPost]
        [Route(RouteConfigs.UsernameExistsRoute)]
        public IHttpActionResult IsUsernameExists([FromBody]string userName)
        {
            //checks if the username exists
            return Ok(usersManager.IsUsernameExists(userName));
        }
        [HttpPost]
        [Route(RouteConfigs.GetTokenRoute)]
        public string GetToken([FromBody]string id)
        {
            //returns a token on user's registeration/login
            Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(RouteConfigs.GetTokenInsideRoute, id);
            if (returnTuple.Item2 == HttpStatusCode.OK)
            {
                return returnTuple.Item1.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        [HttpPost]
        [Route(RouteConfigs.ValidateTokenRoute)]
        public bool ValidateToken([FromBody]string token)
        {
            //validates the token and updates it upon use
            Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(RouteConfigs.ValidateTokenInsideRoute, token);
            if (returnTuple.Item2 == HttpStatusCode.OK)
            {
                return Convert.ToBoolean(returnTuple.Item1);
            }
            else
            {
                return false;
            }
        }
        [HttpPost]
        [Route(RouteConfigs.GetMyUserRoute)]
        public IHttpActionResult GetUserByToken([FromBody]string token)
        {
            //returns the users that matches this token
            Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(RouteConfigs.GetUserIdByTokenRoute, token);
            if (returnTuple.Item2 == HttpStatusCode.OK)
            {
                return Ok(usersManager.GetUserById(returnTuple.Item1.ToString()));
            }
            else
            {
                return Conflict();
            }

        }
    }
}
