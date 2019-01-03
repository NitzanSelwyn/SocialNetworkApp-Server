using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Common.Configs;
using Common.Models;
using SocialProjectServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

        static AmazonS3Client s3Client = new AmazonS3Client();

        private const string bucketName = "socialprojectimages";
        private const string keyName = "";
        private const string filePath = "";
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.EUWest2;
        //     private static IAmazonS3 s3Client;

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
        public async void AddNewPost()
        {
            //TODO
            Post post = new Post
            {
                Author = "nitzan",
                Content = "amalh",
                ImageLink = @"C:\Users\Nitzan's PC\Downloads\MEME\7qyzofpyfcv01.jpg",
                Like = 12
            };

            _post["Author"] = post.Author;
            _post["Content"] = post.Content;
            _post["Likes"] = post.Like;
            _post["Image"] = UploadFileAsync(post.ImageLink,post.ImageLink);

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

        private static string UploadFileAsync(string filePath,string fileName)
        {
            s3Client = new AmazonS3Client(bucketRegion);

            s3Client.PutACL(new PutACLRequest
            {
                BucketName = "socialprojectimages",
                Key = "name",
                CannedACL = S3CannedACL.PublicRead,
            });

            try
            {

                PutObjectRequest putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileName,
                    FilePath = filePath,
                    ContentType = "text/plain"
                };

                var response = s3Client.PutObject(putRequest);

                return "File Location";

            }
            
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            return null ;

        }

    }
}

