using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        List<UserRepresentation> GetBlockedUsers(string userId);
    }
}
