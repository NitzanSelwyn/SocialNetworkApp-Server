using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Databases
{
    public interface INetworkDatabase
    {
        Table GetPostsTable();
        Table GetCommentTable();
        Table GetUsersTable();
    }
}
