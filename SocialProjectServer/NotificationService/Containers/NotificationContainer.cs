using Common.Contracts;
using Common.Contracts.Services;
using NotificationService.Services;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotificationService.Containers
{
    public static class NotificationContainer
    {
        public static readonly Container container;
        static NotificationContainer()
        {
            if (container == null)
            {
                container = new Container();
                container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

                //Services
                container.Register<INotificateService, NotifcateService>(Lifestyle.Singleton);
                container.Register<IHttpClient, HttpClientSender>(Lifestyle.Singleton);
               
            }

        }
    }
}