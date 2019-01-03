using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Common.Configs;
using Common.Models;
using SocialProjectServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SocialProjectServer.Controllers
{
    public class PostsController : ApiController
    {
        AmazonDynamoDBConfig clientConfig;
        AmazonDynamoDBClient client;
        CreateTableResponse response;
        Table usersTable;
        Document _post;

        public PostsController()
        {
            clientConfig = new AmazonDynamoDBConfig();

            client = new AmazonDynamoDBClient(clientConfig);
            string tableName = "Posts";
            var request = new CreateTableRequest
            {
                AttributeDefinitions = new List<AttributeDefinition>(),
                TableName = tableName
            };

             response = client.CreateTable(request);
             usersTable = Table.LoadTable(client, tableName);

             _post = new Document();

        }

        [HttpPost]
        [Route(RouteConfigs.PostNewMessage)]
        public void AddNewPost([FromBody]Post post)
        {
            //TODO
            _post["Author"] = post.Author;


            usersTable.PutItem(_post);


        }

        [HttpGet]
        [Route(RouteConfigs.GetUsersPosts)]
        public List<Post> GetUserPosts([FromBody]User user)
        {
            //TODO
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route(RouteConfigs.GetFolowersPosts)]
        public List<Post> GetFolowersPosts([FromBody]User user)
        {
            //TODO
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route(RouteConfigs.EditPost)]
        public bool EditPost([FromBody]Post post)
        {
            //TODO
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route(RouteConfigs.DeletePost)]
        public bool DeletePost([FromBody]Post post)
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
