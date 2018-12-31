using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Services
{
    class DynamoDBService 
    {
        DynamoDBContext dynamoDBContext;
        AmazonDynamoDBClient dynamoDBClient;

        public DynamoDBService()
        {
            dynamoDBClient = new AmazonDynamoDBClient();

            dynamoDBContext = new DynamoDBContext(dynamoDBClient, new DynamoDBContextConfig
            {
                ConsistentRead = true,
                SkipVersionCheck = true
            
            });
        }
    }
}
