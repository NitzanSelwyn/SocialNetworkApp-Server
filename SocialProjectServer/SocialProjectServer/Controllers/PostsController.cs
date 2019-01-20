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

        [HttpGet]
        [Route(RouteConfigs.GetUsersPosts + "/{userName}/{skipCount}")]
        public List<Post> GetUserPosts(string userName, int skipCount)
        {
            return postManager.GetUserPosts(userName, skipCount);
        }

        [HttpGet]
        [Route(RouteConfigs.GetFolowersPosts + "/{userName}/{skipCount}")]
        public List<Post> GetFolowersPosts(string userName,int skipCount)
        {
            return postManager.GetFolowersPosts(userName,skipCount);
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
            Post returnPost = postManager.LikePost(like);
            if (returnPost != null)
            {
                return Ok(returnPost);
            }
            else
            {
                return Conflict();
            }
        }

        [HttpPost]
        [Route(RouteConfigs.UnLike)]
        public IHttpActionResult UnLikePost([FromBody]Like like)
        {
            Post returnPost = postManager.UnLikePost(like);
            if (returnPost != null)
            {
                return Ok(returnPost);
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
            Post returnPost = postManager.CommentOnPos(comment);
            if (returnPost != null)
            {
                return Ok(returnPost);
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

