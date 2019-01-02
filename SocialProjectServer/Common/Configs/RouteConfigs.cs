﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Configs
{
    public static class RouteConfigs
    {
        public const string GetTokenRoute = "Api/Auth/GetToken";
        public const string ValidateToken = "Api/Auth/ValidateToken";

        public const string UserLogin = "Api/Users/UserLogin";
        public const string UserRegister = "Api/Users/UserRegister";
        public const string UsernameExists = "Api/Users/UsernameExists";
    }
}
