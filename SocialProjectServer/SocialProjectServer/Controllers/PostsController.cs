using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Common.Configs;
using Common.Contracts;
using Common.Contracts.Managers;
using Common.Enums;
using Common.Models;
using Common.Models.TempModels;
using DAL.Databases;
using SocialProjectServer.Containers;
using SocialProjectServer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace SocialProjectServer.Controllers
{
    public class PostsController : ApiController
    {
        IPostManager postManager { get; set; }
        IHttpClient httpClient { get; set; }

        public PostsController()
        {
            postManager = ServerContainer.container.GetInstance<IPostManager>();
            httpClient = ServerContainer.container.GetInstance<IHttpClient>();
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
        public List<Post> GetFolowersPosts(string userName, int skipCount)
        {
            return postManager.GetFolowersPosts(userName, skipCount);
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
                UserRequestModel request = new UserRequestModel(like.UserName, returnPost.Author, "", UserRequestEnum.LikedPost, returnPost.PostId);
                SendNotificationToSerivce(request);
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
                UserRequestModel request = new UserRequestModel(comment.CommenterName, returnPost.Author, "", UserRequestEnum.Commented_On_Post, returnPost.PostId);
                SendNotificationToSerivce(request);
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
        private void SendNotificationToSerivce(UserRequestModel requestModel)
        {
            //Sends the new notifcation to the service via the hub
            if (requestModel.requestType == UserRequestEnum.Follow)
            {
                Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(MainConfigs.NotificateServiceUrl, RouteConfigs.PassNotificationToServiceRoute, requestModel);
                if (returnTuple.Item2 == HttpStatusCode.OK)
                {
                    //Great
                }
                else
                {
                    //log to errors
                }
            }

        }
    }
}

