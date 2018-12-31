using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialProjectServer.Models
{
    [DynamoDBTable("User")]
    public class User
    {
        [DynamoDBHashKey]
        public string UserName { get; set; }

        [DynamoDBRangeKey]
        public string Password { get; set; }

        [DynamoDBProperty]
        public string WorkPlace { get; set; }
    }
}