using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Common.Configs;
using Common.Contracts.Managers;
using Common.Enums;
using Common.Models;
using DAL.Databases;
using SocialProjectServer.Containers;
using SocialProjectServer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;

namespace SocialProjectServer.Controllers
{
    public class PostsController : ApiController
    {
        IPostManager postManager { get; set; }

        public PostsController()
        {
            postManager = ServerContainer.container.GetInstance<IPostManager>();
        }

        [HttpPost]
        [Route(RouteConfigs.PostNewMessage)]
        public IHttpActionResult AddNewPost([FromBody]Post post)
        {
            ResponseEnum response = postManager.AddNewPost(post);
            if (response == ResponseEnum.Succeeded)
            {
                return Ok();
            }
            else
            {
                return Conflict();
            }
        }

        [HttpPost]
        [Route(RouteConfigs.GetUsersPosts)]
        public List<Post> GetUserPosts([FromBody]string userName)
        {
            return postManager.GetUserPosts(userName);
        }

        [HttpPost]
        [Route(RouteConfigs.GetFolowersPosts)]
        public List<Post> GetFolowersPosts([FromBody]string userName)
        {
            return postManager.GetFolowersPosts(userName);
        }

        [HttpDelete]
        [Route(RouteConfigs.DeletePost)]
        public IHttpActionResult DeletePost([FromBody]string postId)
        {
            ResponseEnum response = postManager.DeletePost(postId);

            if (response == ResponseEnum.Succeeded)
            {
                return Ok();
            }
            else
            {
                return Conflict();
            }
        }

        [HttpPost]
        [Route(RouteConfigs.Like)]
        public IHttpActionResult LikePost([FromBody]Like like)
        {
            ResponseEnum response = postManager.LikePost(like);
            if (response == ResponseEnum.Succeeded)
            {
                return Ok();
            }
            else
            {
                return Conflict();
            }
        }

        [HttpPost]
        [Route(RouteConfigs.CommentOnPost)]
        public IHttpActionResult CommentOnPost([FromBody]Comment comment)
        {
            ResponseEnum response = postManager.CommentOnPos(comment);
            if (response == ResponseEnum.Succeeded)
            {
                return Ok();
            }
            else
            {
                return Conflict();
            }
        }

        [HttpPost]
        [Route(RouteConfigs.GetPostsComments)]
        public List<Comment> GetPostsComments([FromBody]string postId)
        {
            return postManager.GetPostsComments(postId);
        }
    }
}

