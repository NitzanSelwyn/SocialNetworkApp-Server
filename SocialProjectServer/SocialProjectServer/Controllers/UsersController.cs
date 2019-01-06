using Common.Configs;
using Common.Contracts;
using Common.Models.TempModels;
using Common.ResponseModels;
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
        public UsersController()
        {
            usersManager = ServerContainer.container.GetInstance<IUsersManager>();
            httpClient = ServerContainer.container.GetInstance<HttpClientSender>();
        }

        [HttpPost]
        [Route(RouteConfigs.UserLogin)]
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
     [Route(RouteConfigs.UserRegister)]
     public IHttpActionResult UserRegister([FromBody]UserRegister userRegister)
     {
            //tries a new user's registration
            return Ok();
     }
     [HttpPost]
     [Route(RouteConfigs.UsernameExists)]
     public IHttpActionResult IsUsernameExists([FromBody]string userName)
        {
            //checks if the username exists
            return Ok(usersManager.IsUsernameExists(userName));
        }
        public string GetToken(string id)
        {
            //returns a token on user's registeration/login
            Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(RouteConfigs.GetTokenRoute, id);
            if (returnTuple.Item2 == HttpStatusCode.OK)
            {
                return returnTuple.Item1.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        public bool ValidateToken(string token)
        {
            //validates the token and updates it upon use
            Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(RouteConfigs.ValidateToken, token);
            if (returnTuple.Item2 == HttpStatusCode.OK)
            {
                return Convert.ToBoolean(returnTuple.Item1);
            }
            else
            {
                return false;
            }
        }
    }
}
