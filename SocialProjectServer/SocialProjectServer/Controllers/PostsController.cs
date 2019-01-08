using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Common.Configs;
using Common.Models;
using DAL.Databases;
using SocialProjectServer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;

namespace SocialProjectServer.Controllers
{
    public class PostsController : ApiController
    {
        static AmazonS3Client s3Client = new AmazonS3Client();

        private const string bucketName = "socialprojectimages";
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.EUWest2;
        private const string baseURL = "https://s3-eu-west-1.amazonaws.com/socialprojectimages/";

        [HttpPost]
        [Route(RouteConfigs.PostNewMessage)]
        public void AddNewPost([FromBody]Post post)
        {
            post = new Post();
            post.Author = "Hello";
            post.Content = "World";
            post.Like = 69;
            post.ImageLink = "asdasda";

           // var imageLink = UploadFileAsync(post.ImageLink, post.Author);
            using (var graphContext = new Neo4jDB("bolt://localhost:7687", "hello", "123456"))
            {
                graphContext.UploadPost(post);
            }
        }

        //[HttpGet]
        //[Route(RouteConfigs.GetUsersPosts)]
        //public List<Post> GetUserPosts([FromBody]User user)
        //{
        //    using (var graphContext = new Neo4jDB("bolt://localhost:7687", "neo4j", "password"))
        //    {
        //        return graphContext.GetUserPosts(user);
        //    }
        //}

        [HttpGet]
        [Route(RouteConfigs.GetFolowersPosts)]
        public List<Post> GetFolowersPosts([FromBody]User user)
        {
            using (var graphContext = new Neo4jDB("bolt://localhost:7687", "neo4j", "password"))
            {
                return graphContext.GetFolowersPosts(user);
            }
        }

        [HttpDelete]
        [Route(RouteConfigs.DeletePost)]
        public void DeletePost([FromBody]Post post)
        {
            using (var graphContext = new Neo4jDB("bolt://localhost:7687", "neo4j", "password"))
            {
                graphContext.DeletePost(post);
            }
        }

        private static string UploadFileAsync(string filePath, string authorName)
        {
            var fileName = $"{authorName}/{DateTime.Now.ToString()}";
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
                    ContentType = "image/png"

                };

                s3Client.PutObject(putRequest);
                return baseURL + fileName;
            }

            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            return null;

        }
    }
}

