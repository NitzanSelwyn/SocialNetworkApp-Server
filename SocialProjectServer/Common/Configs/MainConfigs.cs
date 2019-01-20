using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Configs
{
    public static class MainConfigs
    {
        public const int TokenTTL = 15;// the tokens time to live in minutes
        public const string AuthServiceUrl = "http://localhost:55569";
        public const string NotificateServiceUrl = "http://localhost:50500";
        public const string ServerUrl = "http://localhost:55620";

    }
}
