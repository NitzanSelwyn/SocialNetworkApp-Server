using Common.Configs;
using Common.Contracts;
using Common.Contracts.Managers;
using Common.Enums;
using Common.Models.TempModels;
using SocialProjectServer.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SocialProjectServer.Controllers
{
    public class SettingsController : ApiController
    {
        ISettingsManager settingsManager { get; set; }
        IHttpClient httpClient { get; set; }
      
        public SettingsController()
        {
            settingsManager = ServerContainer.container.GetInstance<ISettingsManager>();
            httpClient = ServerContainer.container.GetInstance<IHttpClient>();
          
        }

        [HttpPost]
        [Route(RouteConfigs.ManageRequestRoute)]
        public IHttpActionResult ManageRequest([FromBody]UserRequestModel requestModel)
        {
            //manages all the user to user request (block/unblock/friend/unfriend...)
            ResponseEnum response = settingsManager.ManageRequest(requestModel);
            if(response == ResponseEnum.Succeeded)
            {
                SendNotificationToSerivce(requestModel);
                return Ok();
            }
            else
            {
                return Conflict();
            }
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

        [HttpPost]
        [Route(RouteConfigs.EditUserPasswordRoute)]
        public IHttpActionResult ChangePassword([FromBody]EditPassword editPassword)
        {
            //edits a user password
            ResponseEnum response = settingsManager.ChangePassword(editPassword);
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
        [Route(RouteConfigs.GetBlockedUsers)]
        public IHttpActionResult GetBlockedUsers([FromBody]string username)
        {
            //returns my blocked users
            return Ok(settingsManager.GetBlockedUsers(username));
        }

        [HttpPost]
        [Route(RouteConfigs.GetFollowingUsers)]
        public IHttpActionResult GetFollowingUsers([FromBody]string username)
        {
            //returns my blocked users
            return Ok(settingsManager.GetFollowingUsers(username));
        }
        [HttpPost]
        [Route(RouteConfigs.GetUsersThatFollowsMe)]
        public IHttpActionResult GetUsersThatFollowsMe([FromBody]string username)
        {
            //returns the users that follows me
            return Ok(settingsManager.GetUsersThatFollowsMe(username));
        }
    }
}
