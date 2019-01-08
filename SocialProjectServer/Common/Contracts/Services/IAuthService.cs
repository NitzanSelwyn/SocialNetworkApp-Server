using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Contracts
{
    public interface IAuthService
    {
        string GetNewToken(string userId);
        bool IsTokenValid(string token);
         string GetUserId(string token);
       
    }
}
