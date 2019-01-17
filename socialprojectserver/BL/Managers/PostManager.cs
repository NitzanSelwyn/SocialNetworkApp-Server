using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Common.Contracts;
using Common.Contracts.Managers;
using Common.Enums;
using Common.Models;
using DAL.Databases;
using System;
using System.Collections.Generic;
using System.IO;

namespace BL.Managers
{
    public class PostManager : IPostManager
    {
        static AmazonS3Client s3Client = new AmazonS3Client();
        public INetworkRepository repository { get; set; }
        private const string bucketName = "socialprojectimages";
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.EUWest1;
        private const string baseURL = "https://s3-eu-west-1.amazonaws.com/socialprojectimages/";
        private const string neo4jDBConnectionString = "bolt://ec2-34-245-150-157.eu-west-1.compute.amazonaws.com:7687";
        private const string neo4jDBUserName = "neo4j";
        private const string neo4jDBPassword = "123456";

        public PostManager(INetworkRepository repository)
        {
            this.repository = repository;
        }

        public ResponseEnum AddNewPost(Post post)
        {
            if (post.Image != null)
            {
                var imageLink = UploadFile(post.Image, post.Author);
                post.ImageLink = imageLink;
            }

            post.PostId = repository.GetLastPostIdAndUpdate();

            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
              return  graphContext.UploadPost(post);
            }
        }

        public ResponseEnum RegisterUserToNeo4j(string userName, string firstName,string lastName)
        {
            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
                return graphContext.RegisterUserToNeo4j(userName, firstName,lastName);
            }
        }

        public ResponseEnum CommentOnPos(Comment comment)
        {
            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
               return graphContext.CommentOnPost(comment);
            }
        }

        public ResponseEnum DeletePost(string postId)
        {
            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
               return graphContext.DeletePost(postId);
            }
        }

        public List<Post> GetFolowersPosts(string userName)
        {
            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
                return graphContext.GetFollowingsPosts(userName);
            }
        }

        public List<Comment> GetPostsComments(string postId)
        {
            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
                return graphContext.GetCommentsOfPost(postId);
            }
        }

        public List<Post> GetUserPosts(string userName)
        {
            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
                return graphContext.GetUserPosts(userName);
            }
        }

        public ResponseEnum LikePost(Like like)
        {
            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
               return graphContext.LikePost(like);
            }
        }

        public string UploadFile(byte[] file, string authorName)
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

        public ResponseEnum UnLikePost(Like like)
        {
            using (var graphContext = new Neo4jDB(neo4jDBConnectionString, neo4jDBUserName, neo4jDBPassword))
            {
                return graphContext.UnLikePost(like);
            }
        }

    }
}
