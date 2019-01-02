using BL.Managers;
using Common.Contracts;
using DAL;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using SocialProjectServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialProjectServer.Containers
{
    public static class ServerContainer
    {
        public static readonly Container container;
        static ServerContainer()
        {
            if (container == null)
            {
                container = new Container();
                container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
                container.Register<IHttpClient, HttpClientSender>(Lifestyle.Singleton);
                container.Register<INetworkRepository, NetworkRepository>(Lifestyle.Singleton);
                container.Register<IUsersManager, UsersManager>(Lifestyle.Singleton);
            }

        }
    }
}