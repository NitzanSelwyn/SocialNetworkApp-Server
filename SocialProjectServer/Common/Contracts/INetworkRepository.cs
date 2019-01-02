using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialProjectServer.Models;

namespace Common.Contracts
{
    public interface INetworkRepository
    {
        User GetUserById(string id);
    }
}
