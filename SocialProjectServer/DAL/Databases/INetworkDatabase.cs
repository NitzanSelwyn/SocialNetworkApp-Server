using Amazon.DynamoDBv2.DocumentModel;
using Common.Enums;
using Common.Models.TempModels;
using SocialProjectServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Contracts.Databases
{
    public interface INetworkDatabase
    {
        Table GetPostsTable();
        Table GetCommentTable();
        Table GetUsersTable();
    }
}
