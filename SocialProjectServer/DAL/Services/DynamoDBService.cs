using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Services
{
    public class DynamoDBService
    {
        private readonly DynamoDBContext dynamoDBContext;
        private readonly AmazonDynamoDBClient dynamoDBClient;

        public DynamoDBService()
        {
            dynamoDBClient = new AmazonDynamoDBClient();

            dynamoDBContext = new DynamoDBContext(dynamoDBClient, new DynamoDBContextConfig
            {
                ConsistentRead = true,
                SkipVersionCheck = true
            });
        }

        //public void Login(string userName, string password)
        //{
        //     dynamoDBContext.Query(hashKeyValue: userName);
        //}
    }
}