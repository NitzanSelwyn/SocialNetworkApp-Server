using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Common.Configs;
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
        private Neo4jDB neo4JDB;

        public PostManager(INetworkRepository repository)
        {
            this.repository = repository;
            neo4JDB = new Neo4jDB();
        }

        public ResponseEnum AddNewPost(Post post)
        {
            if (post.Image != null)
            {
                var imageLink = UploadFile(post.Image, post.Author);
                post.ImageLink = imageLink;
            }

            post.PostId = repository.GetLastPostIdAndUpdate();

            return neo4JDB.UploadPost(post);
        }

        public ResponseEnum RegisterUserToNeo4j(string userName, string firstName, string lastName)
        {
            return neo4JDB.RegisterUserToNeo4j(userName, firstName, lastName);
        }

        public Post CommentOnPos(Comment comment)
        {
            return neo4JDB.CommentOnPost(comment);
        }

        public ResponseEnum DeletePost(string postId)
        {
            return neo4JDB.DeletePost(postId);
        }

        public List<Post> GetFolowersPosts(string userName, int skipNuber)
        {
            return neo4JDB.GetFollowingsPosts(userName,skipNuber);
        }

        public List<Comment> GetPostsComments(string postId)
        {
            return neo4JDB.GetCommentsOfPost(postId);
        }

        public List<Post> GetUserPosts(string userName, int skipNuber)
        {
            return neo4JDB.GetUserPosts(userName,skipNuber);
        }

        public Post LikePost(Like like)
        {
            return neo4JDB.LikePost(like);
        }

        public Post UnLikePost(Like like)
        {
            return neo4JDB.UnLikePost(like);
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


    }
}
