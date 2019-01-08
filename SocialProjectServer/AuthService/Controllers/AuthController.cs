using AuthService.Containers;
using Common.Configs;
using Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AuthService.Controllers
{
    public class AuthController : ApiController
    {
        IAuthService authService { get; set; }
        public AuthController()
        {
            authService = AuthContainer.container.GetInstance<IAuthService>();
        }
        [Route(RouteConfigs.GetTokenInsideRoute)]
        [HttpPost]
        public string GetToken([FromBody]string userId)
        {
            //returns a new Token
            return authService.GetNewToken(userId);
        }

        [Route(RouteConfigs.ValidateTokenInsideRoute)]
        public bool IsTokenValid([FromBody]string token)
        {
            //validates and update the token's last used
            return authService.IsTokenValid(token);
        }
        [HttpPost]
        [Route(RouteConfigs.GetUserIdByTokenRoute)]
        public string GetUsernameByToken([FromBody]string token)
        {
            //returns the userid that matches this token
            return authService.GetUserId(token);
        }
    }
}
