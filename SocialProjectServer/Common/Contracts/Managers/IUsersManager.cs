using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models.TempModels;
using SocialProjectServer.Models;

namespace Common.Contracts
{
    public interface IUsersManager
    {
        User TryLogin(UserLogin userLogin);
        bool IsUsernameExists(string userName);
    }
}
