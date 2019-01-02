using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialProjectServer.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(clientConfig);
            string tableName = "user";
            Table usersTable = Table.LoadTable(client, tableName);
            Document d = new Document();
            d["name"] = "shahafd";
            d["username"] = "shahafd94";
            usersTable.PutItem(d);
            var item = usersTable.GetItem("shahafd");
            return item["username"];
        }
    }
}