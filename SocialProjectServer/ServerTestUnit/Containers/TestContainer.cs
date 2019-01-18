
using Common.Contracts;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTestUnit.Containers
{
   public static class TestContainer
    {
        public static readonly Container container;
        static TestContainer()
        {
            if (container == null)
            {
                container = new Container();
                container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
                container.Register<IAuthService, AuthService.AuthService>(Lifestyle.Singleton);
            }

        }
    }
}
