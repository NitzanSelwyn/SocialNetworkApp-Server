using Common.Contracts;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuthService.Containers
{
    public static class AuthContainer
    {
        public static readonly Container container;
        static AuthContainer()
        {
            if (container == null)
            {
                container = new Container();
                container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
                container.Register<IAuthService, AuthService>(Lifestyle.Singleton);
            }

        }
    }
}