
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
                case UserRequestEnum.UnBlock:
                    return repository.UnBlockUser(request.userId, request.onUserId);
                case UserRequestEnum.Follow:
                    return repository.FollowUser(request.userId, request.onUserId);
                case UserRequestEnum.UnFollow:
                    return repository.UnFollowUser(request.userId, request.onUserId);
                default:
                    return ResponseEnum.Succeeded;
            }
        }

        public List<UserRepresentation> GetBlockedUsers(string userId)
        {
            //returns all the users that this user blocked
            return repository.GetBlockedUsers(userId);
        }

        public List<UserRepresentation> GetFollowingUsers(string userId)
        {
            return repository.GetFollowingUsers(userId);
        }

        public ResponseEnum ChangePassword(EditPassword editPassword)
        {
            return repository.EditPassword(editPassword);
        }
        public List<UserRepresentation> GetUsersThatFollowsMe(string userId)
        {
            //returns that users that follows me
            return repository.GetUsersThatFollowsMe(userId);
        }
    }
}
