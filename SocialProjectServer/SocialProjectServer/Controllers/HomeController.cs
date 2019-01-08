using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Common.Contracts;
using SocialProjectServer.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialProjectServer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var db = ServerContainer.container.GetInstance<INetworkRepository>();
            return View();
        }
    }
}