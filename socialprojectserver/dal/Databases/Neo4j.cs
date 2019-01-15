using Common.Enums;
using Common.Models;
using Common.Models.TempModels;
using DAL.Repostiories;
using Neo4j.Driver.V1;
using Newtonsoft.Json;
using SocialProjectServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Databases
{
    public class Neo4jDB : IDisposable
    {
        private readonly IDriver _driver;

        public Neo4jDB(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));

        }

        public void Dispose()
        {
            _driver?.Dispose();
        }

        public ResponseEnum UploadPost(Post post)
        {

            var statment = $"MATCH (u:User)" +
                           $"WHERE u.Username = \"{post.Author}\"" +
                           $"CREATE (p:Post {{Author: \"{post.Author}\", Content: \"{post.Content}\", ImageLink: \"{post.ImageLink}\", FullName: \"{post.FullName}\"}})" +
                           $"CREATE (u)-[:Posted]->(p)" +
                           $"RETURN *";
            try
            {
                using (var session = _driver.Session())
                {
                    var results = session.Run(statment).Consume();
                }
                return ResponseEnum.Succeeded;
            }
            catch (Exception)
            {

                return ResponseEnum.Failed;
            }
        }

        public List<Post> GetUserPosts(string userName)
        {
            List<Post> postList = new List<Post>();

            var statment = $"MATCH (u:User)-[:Posted]->(p:Post)" +
                           $"WHERE u.Username = \"{userName}\"" +
                           $"RETURN p ORDER BY p.DatePosted DESC";

            using (var session = _driver.Session())
            {
                var results = session.Run(statment);

                foreach (var result in results)
                {
                    var nodeProps = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    postList.Add(JsonConvert.DeserializeObject<Post>(nodeProps));
                }
                return postList;
            }
        }

        public List<Post> GetFollowingsPosts(string userName)
        {
            List<Post> postList = new List<Post>();

            var statment = $"MATCH (u:User)-[:Follow]->(u2:User)-[:Posted]->(p:Post)" +
                           $"WHERE u.Username = \"{userName}\"" +
                           $"MATCH (u)-[b:Blocked]->[u2]" +
                           $"AMD NOT EXISTS ((u)-[:Blocked]-(u2))" +                         
                           $"RETURN p";

            using (var session = _driver.Session())
            {
                var results = session.Run(statment);

                foreach (var result in results)
                {
                    var nodeProps = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    postList.Add(JsonConvert.DeserializeObject<Post>(nodeProps));
                }
                return postList;
            }
        }

        public ResponseEnum DeletePost(string postId)
        {
            var statment = $"MATCH (p:Post)" +
                           $"WHERE p.PostId = \"{postId}\"" +
                           $"DETACH DELETE p";

            try
            {
                using (var session = _driver.Session())
                {
                    var results = session.Run(statment).Consume();
                }

                return ResponseEnum.Succeeded;
            }
            catch (Exception)
            {

                return ResponseEnum.Failed;
            }
        }

        public ResponseEnum RegisterUserToNeo4j(string userName)
        {
            var statment = $"CREATE (u:User {{Username: \"{userName}\"}})";

            try
            {
                using (var session = _driver.Session())
                {
                    var results = session.Run(statment).Consume();
                }

                return ResponseEnum.Succeeded;
            }
            catch (Exception)
            {

                return ResponseEnum.Failed;
            }
        }

        public ResponseEnum LikePost(Like like)
        {
            var statment = $"MATCH (p:Post)" +
                           $"WHERE p.postId = \"{like.postId}\"" +
                           $"MERGE (u:User)-[:Like]->(p)" +
                           $"WHERE u.Username = \"{like.UserName}\"" +
                           $"RETURN *";
            try
            {

                using (var session = _driver.Session())
                {
                    var results = session.Run(statment).Consume();
                }

                return ResponseEnum.Succeeded;
            }
            catch (Exception)
            {

                return ResponseEnum.Failed;
            }
        }

        public ResponseEnum CommentOnPost(Comment comment)
        {
            var statment = $"MATCH (p:Post)" +
                           $"WHERE p.postId = \"{comment.postId}\"" +
                           $"CREATE (c:Comment {{Content: \"{comment.Text}\", Author: \"{comment.CommenterName}\", Date: {comment.CommentedDate}}})-[:CommentedOn]->(p)" +
                           $"WHERE u.Username = \"{comment.CommenterName}\"" +
                           $"CREATE (u)-[:Comment]->(c)" +
                           $"RETURN *";

            try
            {
                using (var session = _driver.Session())
                {
                    var results = session.Run(statment).Consume();
                }

                return ResponseEnum.Succeeded;
            }
            catch (Exception)
            {

                return ResponseEnum.Failed;
            }
        }

        public ResponseEnum BlockUser(string userName, string blockedUserName)
        {
            var statment = $"MATCH (u:User)" +
               $"WHERE u.Username = \"{userName}\"" +
               $"MATCH (bu:User)" +
               $"WHERE bu.Username = \"{blockedUserName}\"" +
               $"MERGE (u)-[:Block]->(bu)" +
               $"RETURN *";

            try
            {
                using (var session = _driver.Session())
                {
                    var results = session.Run(statment);
                }
                return ResponseEnum.Succeeded;
            }
            catch (Exception)
            {

                return ResponseEnum.Failed;
            }
        }

        public ResponseEnum UnBlockUser(string userName, string unBlockedUserName)
        {
            var statment = $"MATCH (u:User)-[b:Block]->(bu:User)" +
                           $"WHERE u.Username = \"{userName}\" AND bu.Username = \"{unBlockedUserName}\"" +
                           $"DELETE b";

            try
            {
                using (var session = _driver.Session())
                {
                    var results = session.Run(statment);
                }
                return ResponseEnum.Succeeded;
            }
            catch (Exception)
            {

                return ResponseEnum.Failed;
            }
        }

        public ResponseEnum FollowUser(string userName, string UserToFollow)
        {
            var statment = $"MATCH (u:User)" +
               $"WHERE u.Username = \"{userName}\"" +
               $"MATCH (bu:User)" +
               $"WHERE bu.Username = \"{UserToFollow}\"" +
               $"MERGE (u)-[:Follow]->(bu)" +
               $"RETURN *";

            try
            {
                using (var session = _driver.Session())
                {
                    var results = session.Run(statment);
                }
                return ResponseEnum.Succeeded;
            }
            catch (Exception)
            {

                return ResponseEnum.Failed;
            }
        }

        public ResponseEnum UnFollowUser(string userName, string unFollowUserName)
        {
            var statment = $"MATCH (u:User)-[f:Follow]->(bu:User)" +
                           $"WHERE u.Username = \"{userName}\" AND bu.Username = \"{unFollowUserName}\"" +
                           $"DELETE f";

            try
            {
                using (var session = _driver.Session())
                {
                    var results = session.Run(statment);
                }
                return ResponseEnum.Succeeded;
            }
            catch (Exception)
            {

                return ResponseEnum.Failed;
            }
        }

        public List<string> GetBlockedUsers(string userName)
        {
            List<string> usertList = new List<string>();

            var statment = $"MATCH (u:User)-[:Block]->(bu:User)" +
                           $"WHERE u.Username = \"{userName}\"" +
                           $"RETURN bu";

            using (var session = _driver.Session())
            {
                var results = session.Run(statment);

                foreach (var result in results)
                {
                    var nodeProps = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    usertList.Add(JsonConvert.DeserializeObject<string>(nodeProps));
                }
                return usertList;
            }

        }

        public List<string> GetFollowingUsers(string userName)
        {
            List<string> usertList = new List<string>();

            var statment = $"MATCH (u:User)-[:Follow]->(fu:User)" +
                           $"WHERE u.Username = \"{userName}\"" +
                           $"RETURN fu";

            using (var session = _driver.Session())
            {
                var results = session.Run(statment);

                foreach (var result in results)
                {
                    var nodeProps = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    usertList.Add(JsonConvert.DeserializeObject<string>(nodeProps));
                }
                return usertList;
            }
        }

        public List<Comment> GetCommentsOfPost(string postId)
        {
            var commentList = new List<Comment>();

            var statment = $"MATCH (c:Comment)-[:CommentedOn]->(p:Post)" +
                           $"WHERE p.PostId = \"{postId}\"" +
                           $"RETURN c";

            using (var session = _driver.Session())
            {
                var results = session.Run(statment);

                foreach (var result in results)
                {
                    var nodeProps = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    commentList.Add(JsonConvert.DeserializeObject<Comment>(nodeProps));
                }
                return commentList;
            }
        }
    }
}
