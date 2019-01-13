using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Common.Enums;
using Common.Models.TempModels;
using Common.ResponseModels;
using SocialProjectServer.Models;

namespace Common.Contracts
{
    public interface INetworkRepository
    {
        User GetUserById(string id);
        User RegisterUser(UserRegister userRegister);
        ResponseEnum BlockUser(string userId, string onUserId);
        ResponseEnum UnBlockUser(string userId, string onUserId);
        ResponseEnum FollowUser(string userId, string onUserId);
        ResponseEnum UnFollowUser(string userId, string onUserId);

        List<UserRepresentation> GetBlockedUsers(string userId);
        List<UserRepresentation> GetFollowingUsers(string userId);
        Document GetUserDocById(string id);
        User EditUserDetails(User user);
        ResponseEnum ChangePassword(EditPassword editPassword);
    }
}
