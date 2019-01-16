using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Common.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Databases
{
    public class DynamoDB : INetworkDatabase
    {
        AmazonDynamoDBConfig clientConfig { get; set; }
        AmazonDynamoDBClient client { get; set; }
        Table usersTable { get; set; }
        Table configsTable { get; set; }
        public DynamoDB()
        {
            clientConfig = new AmazonDynamoDBConfig();
            client = new AmazonDynamoDBClient(clientConfig);
            usersTable = Table.LoadTable(client, DatabaseConfigs.UsersTable);
            configsTable = Table.LoadTable(client, DatabaseConfigs.PostIdTable);

        }
        public Table GetUsersTable()
        {
            //returns the users table
            return usersTable;
        }

        public Table GetConfigsTable()
        {
            return configsTable;
        }
    }
}
