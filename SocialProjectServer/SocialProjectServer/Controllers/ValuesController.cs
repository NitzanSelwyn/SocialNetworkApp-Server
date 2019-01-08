using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SocialProjectServer.Controllers
{
    public class ValuesController : ApiController
    {
        [Route("api/test")]
        [HttpGet]
        public string Get()
        {
            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
            //    clientConfig.ServiceURL = "http://localhost:8000";
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(clientConfig);
            string tableName = "users";
            var request = new CreateTableRequest
            {
                AttributeDefinitions = new List<AttributeDefinition>()
                ,

                TableName = tableName
            };


            var response = client.CreateTable(request);
            Table usersTable = Table.LoadTable(client, tableName);

            Document user = new Document();
            user["id"] = 1;
            user["name"] = "shahaf";
            user["password"] = "dahan";
            usersTable.PutItem(user);
            GetItemOperationConfig config = new GetItemOperationConfig
            {
                AttributesToGet = new List<string> { "id", "name", "password" },
                ConsistentRead = true
            };
            Document doc = usersTable.GetItem(1, config);
            return PrintDocument(doc);
        }

        private string PrintDocument(Document updatedDocument)
        {
            string returnStr = "";
            foreach (var attribute in updatedDocument.GetAttributeNames())
            {
                string stringValue = null;
                var value = updatedDocument[attribute];
                if (value is Primitive)
                    stringValue = value.AsPrimitive().Value.ToString();
                else if (value is PrimitiveList)
                    stringValue = string.Join(",", (from primitive
                                    in value.AsPrimitiveList().Entries
                                                    select primitive.Value).ToArray());
                returnStr += $"{attribute}- {stringValue} \n";

            }
            return returnStr;
        }
        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}