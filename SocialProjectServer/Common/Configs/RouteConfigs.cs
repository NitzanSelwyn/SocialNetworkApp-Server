using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Configs
{
    public static class RouteConfigs
    {
        public const string GetUserIdByTokenRoute = "Api/Auth/GetUsernameByToken";
        public const string GetTokenInsideRoute = "Api/Auth/GetInsideToken";
        public const string ValidateTokenInsideRoute = "Api/Auth/ValidateInsideToken";
        public const string GetTokenRoute = "Api/Users/GetToken";
        public const string ValidateTokenRoute = "Api/Users/ValidateToken";

        public const string UserLogin = "Api/Users/UserLogin";
        public const string UserRegister = "Api/Users/UserRegister";
        public const string UsernameExists = "Api/Users/UsernameExists";

        public const string PostNewMessage = "Api/Post";
        public const string GetUsersPosts = "Api/Post/UsersPosts";
        public const string GetFolowersPosts = "Api/Post/FolowersPosts";
        public const string EditPost = "Api/Post/edit";
        public const string DeletePost = "Api/Post";
        public const string GetMyUserRoute = "Api/users/GetUserByToken";
        public const string UserLoginRoute = "Api/Users/UserLogin";
        public const string UserRegisterRoute = "Api/Users/UserRegister";
        public const string UsernameExistsRoute = "Api/Users/UsernameExists";
    }
}
