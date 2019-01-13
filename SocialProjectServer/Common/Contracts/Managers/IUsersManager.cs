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
    public interface IUsersManager
    {
        User TryLogin(UserLogin userLogin);
        bool IsUsernameExists(string userName);
        User TryRegister(UserRegister userRegister);
        User GetUserById(string id);
        User EditUserDetails(User user);
        User FacebookLogin(FacebookUser user);
    }
}
