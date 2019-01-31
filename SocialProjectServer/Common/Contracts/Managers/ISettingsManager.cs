using Common.Enums;
using Common.Models.TempModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Contracts.Managers
{
    public interface ISettingsManager
    {
        ResponseEnum ManageRequest(UserRequestModel request);
        ResponseEnum ChangePassword(EditPassword editPassword);
        List<UserRepresentation> GetBlockedUsers(string userId);
        List<UserRepresentation> GetFollowingUsers(string userId);
        List<UserRepresentation> GetUsersThatFollowsMe(string userId);
    }
}
