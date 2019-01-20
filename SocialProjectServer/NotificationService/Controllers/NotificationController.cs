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
        }
        [Route(RouteConfigs.PassNotificationToServiceRoute)]
        public IHttpActionResult PassNotificationFromServer([FromBody]UserRequestModel request)
        {
            //creates a new notfication and passes it to the notfication service
            Notification notification = new Notification(request.userId, request.onUserId, DateTime.Now, NotificationEnum.Followed);

            notificateService.AddNotification(notification,GetUserFullName(request.userId));
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
    }
}
