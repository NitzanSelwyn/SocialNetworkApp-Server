using Common.Configs;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SocialProjectServer.Controllers
{
    public class PostsController : ApiController
    {
        [HttpPost]
        [Route(RouteConfigs.PostNewMessage)]
        public bool AddNewPost([FromBody]Post post)
        {
            //TODO
            throw new NotImplementedException();
 
        }

        [HttpGet]
        [Route(RouteConfigs.GetUsersPosts)]
        public List<Post> GetUserPosts([FromBody]Models.User user)
        {
            //TODO
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route(RouteConfigs.GetFolowersPosts)]
        public List<Post> GetFolowersPosts([FromBody]Models.User user)
        {
            //TODO
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route(RouteConfigs.EditPost)]
        public bool EditPost([FromBody]Post post)
        {
            //TODO
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route(RouteConfigs.DeletePost)]
        public bool DeletePost([FromBody]Post post)
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
