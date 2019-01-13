using Common.Configs;
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
        public SettingsController()
        {
            settingsManager = ServerContainer.container.GetInstance<ISettingsManager>();
        }

        [HttpPost]
        [Route(RouteConfigs.ManageRequestRoute)]
        public IHttpActionResult ManageRequest([FromBody]UserRequestModel requestModel)
        {
            //manages all the user to user request (block/unblock/friend/unfriend...)
            ResponseEnum response = settingsManager.ManageRequest(requestModel);
            if(response == ResponseEnum.Succeeded)
            {
                return Ok();
            }
            else
            {
                return Conflict();
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
    }
}
