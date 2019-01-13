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
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.EUWest1;
        private const string baseURL = "https://s3-eu-west-1.amazonaws.com/socialprojectimages/";
        private const string neo4jDBConnectionString = "bolt://ec2-34-245-150-157.eu-west-1.compute.amazonaws.com:7687";
        private const string neo4jDBUserName = "neo4j";
        private const string neo4jDBPassword = "123456";

        [HttpPost]
        [Route(RouteConfigs.PostNewMessage)]
        public void AddNewPost([FromBody]Post post)
        {
            if (post.Image != null)
            {
                var imageLink = UploadFile(post.Image, post.Author);
                post.ImageLink = imageLink;
            }

            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
                graphContext.UploadPost(post);
            }
        }

        [HttpGet]
        [Route(RouteConfigs.GetUsersPosts)]
        public List<Post> GetUserPosts([FromBody]string userName)
        {
            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
                return graphContext.GetUserPosts(userName);
            }
        }

        [HttpPost]
        [Route(RouteConfigs.GetFolowersPosts)]
        public List<Post> GetFolowersPosts([FromBody]string userName)
        {
            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
                return graphContext.GetFollowingsPosts(userName);
            }
        }

        [HttpDelete]
        [Route(RouteConfigs.DeletePost)]
        public void DeletePost([FromBody]Post post)
        {
            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
                graphContext.DeletePost(post);
            }
        }

        [HttpPost]
        [Route(RouteConfigs.Like)]
        public void LikePost([FromBody]Like like)
        {
            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
                graphContext.LikePost(like);
            }
        }

        [HttpPost]
        [Route(RouteConfigs.CommentOnPost)]
        public void CommentOnPost([FromBody]Comment comment)
        {
            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
                graphContext.CommentOnPost(comment);
            }
        }

        private static string UploadFile(byte[] file, string authorName)
        {
            var image = new MemoryStream(file);

            var fileName = $"{authorName}/{DateTime.Now.ToString()}.png";

            var fileTransferUtility =
                   new TransferUtility(s3Client);

            try
            {
                fileTransferUtility.Upload(stream: image, bucketName: bucketName, key: fileName);

                fileTransferUtility.S3Client.PutACL(new PutACLRequest
                {
                    CannedACL = S3CannedACL.PublicReadWrite,
                    BucketName = bucketName,
                    Key = fileName
                });

                var URL = s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = fileName,
                    Expires = DateTime.MaxValue
                });

                var finalURL = URL.Split('?');
                return finalURL[0];
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

