using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Configs
{
    public static class DatabaseConfigs
    {
        public const string neo4jDBConnectionString = "bolt://ec2-34-245-150-157.eu-west-1.compute.amazonaws.com:7687";
        public const string neo4jDBUserName = "neo4j";
        public const string neo4jDBPassword = "123456";

        public const string UsersTable = "Users";
        public const string UsersKey = "Username";

        public const string PostsTable = "Posts";
        public const string PostsKey = "id";

        public const string CommentsTable = "Comments";
        public const string CommentsKey = "id";
    }
}
