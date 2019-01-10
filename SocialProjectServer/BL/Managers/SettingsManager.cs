using Amazon.DynamoDBv2.DocumentModel;
using Common.Contracts;
using Common.Contracts.Managers;
using Common.Enums;
using Common.Models.TempModels;
using SocialProjectServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Managers
{
    public class SettingsManager : ISettingsManager
    {
        public INetworkRepository repository { get; set; }
        public SettingsManager(INetworkRepository repository)
        {
            this.repository = repository;
        }
        
        public ResponseEnum ManageRequest(UserRequestModel request)
        {
            //manages all the user to user requests (block/unblock/friend/unfriend...)
            switch (request.requestType)
            {
                case UserRequestEnum.Block:
                    return repository.BlockUser(request.userId, request.onUserId);
                //  case UserRequestEnum.UnBlock:
                //      break;
                //  case UserRequestEnum.Follow:
                //      break;
                //  case UserRequestEnum.UnFollow:
                //      break;
                //  case UserRequestEnum.AddToFriends:
                //      break;
                //  case UserRequestEnum.RemoveFromFriends:
                //      break;
                default:
                    return ResponseEnum.Succeeded;
            }
        }
        public List<UserRepresentation> GetBlockedUsers(string userId)
        {
            //returns all the users that this user blocked
            return repository.GetBlockedUsers(userId);
        }
    }
}
