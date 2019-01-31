using BL.Managers;
using Common.Contracts;
using Common.Contracts.Managers;
using DAL;
using DAL.Databases;
using DAL.Repostiories;
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

                //Services
                container.Register<IHttpClient, HttpClientSender>(Lifestyle.Singleton);

                //Databases
                container.Register<INetworkDatabase, DynamoDB>(Lifestyle.Singleton);
                container.Register<INetworkRepository, NetworkRepository>(Lifestyle.Singleton);

                //Managers
                container.Register<IUsersManager, UsersManager>(Lifestyle.Singleton);
                container.Register<ISettingsManager, SettingsManager>(Lifestyle.Singleton);
                container.Register<IPostManager, PostManager>(Lifestyle.Singleton);
            }

        }
    }
}