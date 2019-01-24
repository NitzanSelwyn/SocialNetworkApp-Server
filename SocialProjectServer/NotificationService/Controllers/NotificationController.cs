using Common.Configs;
using Common.Contracts;
using Common.Contracts.Services;
using Common.Enums;
using Common.Models;
using Common.Models.TempModels;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Linq;
using NotificationService.Containers;
using NotificationService.SignalrHubs;
using SocialProjectServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NotificationService.Controllers
{
    public class NotificationController : ApiController
    {
        public INotificateService notificateService { get; set; }
        public IHttpClient httpClient { get; set; }
        IHubContext context { get; set; }
        public NotificationController()
        {
            context = GlobalHost.ConnectionManager.GetHubContext<SocialNetHub>();
            notificateService = NotificationContainer.container.GetInstance<INotificateService>();
            httpClient = NotificationContainer.container.GetInstance<IHttpClient>();
        }
        [Route(RouteConfigs.PassNotificationToServiceRoute)]
        public IHttpActionResult PassNotificationFromServer([FromBody]UserRequestModel request)
        {
            //creates a new notfication and passes it to the notfication service
            string FullName = GetUserFullName(request.userId);
            Notification notification = new Notification();
            switch (request.requestType)
            {
                case UserRequestEnum.Follow:
                    notification = new Notification(request.userId, request.onUserId, FullName, DateTime.Now, NotificationEnum.Followed);
                    break;
                case UserRequestEnum.Commented_On_Post:
                    notification = new Notification(request.userId, request.onUserId, FullName, DateTime.Now, NotificationEnum.Commented_On_Post, request.postId);
                    break;
                case UserRequestEnum.LikedPost:
                    notification = new Notification(request.userId, request.onUserId, FullName, DateTime.Now, NotificationEnum.Liked_Post, request.postId);
                    break;
                case UserRequestEnum.Tagged_In_Post:
                    notification = new Notification(request.userId, request.onUserId, FullName, DateTime.Now, NotificationEnum.Tagged_In_Post, request.postId);
                    break;
                default:
                    break;
            }
            notificateService.AddNotification(notification);
            if (notificateService.userNames.Keys.Contains(request.onUserId))
            {
                context.Clients.Client(notificateService.userNames[request.onUserId]).GetMyNotifications();
            }

            return Ok();
        }
        public string GetUserFullName(string userId)
        {
            //returns the fullname of the user
            Tuple<object, HttpStatusCode> returnTuple = httpClient.PostRequest(MainConfigs.ServerUrl, RouteConfigs.GetUserByUsername);
            if (returnTuple.Item2 == HttpStatusCode.OK)
            {
                JObject jobj = new JObject();
                jobj = (JObject)returnTuple.Item1;
                User user = jobj.ToObject<User>();
                return $"{user.FirstName} {user.LastName}";
            }
            else
            {
                return string.Empty;
            }
        }
        [HttpPost]
        [Route(RouteConfigs.GetNotificationCountInsideRoute)]
        public IHttpActionResult GetNotificationsCount([FromBody]string username)
        {
            if (notificateService.ClientHaveNotifications(username))
            {
                return Ok(notificateService.NotifCollec[username].Count);
            }
            else
            {
                return Conflict();
            }
        }
        [HttpPost]
        [Route(RouteConfigs.GetNotificationsInsideRoute)]
        public IHttpActionResult GetNotifications([FromBody]string username)
        {
            if (notificateService.ClientHaveNotifications(username))
            {
                return Ok(notificateService.NotifCollec[username]);
            }
            else
            {
                return Conflict();
            }
        }
        [HttpPost]
        [Route(RouteConfigs.ClearNotificationsInsideRoute)]
        public IHttpActionResult ClearNotifications([FromBody]string username)
        {
            //clears the notifications for this user
            bool success = notificateService.ClearNotifsForUser(username);
            if (success)
            {
                return Ok();
            }
            else
            {
                return Conflict();
            }
        }
    }
}
