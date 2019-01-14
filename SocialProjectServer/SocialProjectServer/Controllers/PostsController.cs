using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Common.Configs;
using Common.Contracts.Managers;
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
        public void AddNewPost([FromBody]Post post)
        {
            postManager.AddNewPost(post);
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
        public void DeletePost([FromBody]string postId)
        {
            postManager.DeletePost(postId);
        }

        [HttpPost]
        [Route(RouteConfigs.Like)]
        public void LikePost([FromBody]Like like)
        {
            postManager.LikePost(like);
        }

        [HttpPost]
        [Route(RouteConfigs.CommentOnPost)]
        public void CommentOnPost([FromBody]Comment comment)
        {
            postManager.CommentOnPos(comment);
        }

        [HttpPost]
        [Route(RouteConfigs.GetPostsComments)]
        public List<Comment> GetPostsComments([FromBody]string postId)
        {
            return postManager.GetPostsComments(postId);
        }
    }
}

