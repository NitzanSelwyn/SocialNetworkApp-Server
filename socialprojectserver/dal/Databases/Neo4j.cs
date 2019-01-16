﻿using Common.Enums;
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

        /// <summary>
        /// Gets a post from the Client and saveing it to neo4j
        /// every post contains the UserName of the one that posted it
        /// so the user node will be found by the UserName and a new Post Node will be created
        /// Then a Posted Connection will be created from the User node to the new Post Node
        /// </summary>
        /// <param name="post"></param>
        /// <returns> If the upload was a without error will send "ok" to the client </returns>
        public ResponseEnum UploadPost(Post post)
        {

            var statment = $"MATCH (u:User)" +
                           $"WHERE u.Username = \"{post.Author}\"" +
                           $"CREATE (p:Post {{Author: \"{post.Author}\", Content: \"{post.Content}\", ImageLink: \"{post.ImageLink}\", FullName: \"{post.FullName}\", DatePosted: \"{post.DatePosted}\", PostId: \"{post.PostId}\"}})" +
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

        /// <summary>
        /// The client sends a UserName of a user (when entering user profile) and getting all 
        /// his Post in descending order by date
        /// </summary>
        /// <param name="userName"></param>
        /// <returns> List of post from a specific user</returns>
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

        /// <summary>
        /// Gets a UeserName from the client and searching for all of the posts
        /// of the users he follow, the posts that will be return will follow this rules
        /// If you block the user or he blocks you his posts will not be return
        /// </summary>
        /// <param name="userName"></param>
        /// <returns> List of all the post of the users you follow</returns>
        public List<Post> GetFollowingsPosts(string userName)
        {
            List<Post> postList = new List<Post>();

            var statment = $"MATCH (u:User)-[:Follow]->(u2:User)-[:Posted]->(p:Post)" +
                           $"WHERE u.Username = \"{userName}\" AND " +
                           $"NOT EXISTS ((u)-[:Blocked]-(u2))" +
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

        /// <summary>
        /// The Client press on a post he wants to delete on his profile and the
        /// PostId will be send to the server, in the neo4j service the post will be found and all of the 
        /// connections will be detached and then the node will be removed
        /// </summary>
        /// <param name="postId"></param>
        /// <returns> If the delete was a without error will send "ok" to the client </returns>
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

        /// <summary>
        /// When a new user is registering to the website his UserName will be send to this function and a new node will be 
        /// created with his UserName (and if a new users loggin in with facebook his UserName will be also saved)
        /// </summary>
        /// <param name="userName"></param>
        /// <returns> If the RegisterUser was a without error will send "ok" to the client </returns>
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

        /// <summary>
        /// When a user is liking a post a like model will be passed to the server
        /// the post model contains who liked it and on what post.
        /// so a new connection between the user node that liked the post to the post will be formed
        /// </summary>
        /// <param name="like"></param>
        /// <returns> If the like was a without error will send "ok" to the client </returns>
        public ResponseEnum LikePost(Like like)
        {
            var statment = $"MATCH (p:Post)" +
                           $"WHERE p.postId = \"{like.postId}\"" +
                           $"MERGE (u:User)-[:Liked]->(p)" +
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

        /// <summary>
        /// When a user is commenting on a post the comment model will be send to the server
        /// The comment model contains on witch post it was made the user that made it and the comment text.
        /// a new Comment node will be created with the info from the model, also a new connection from the user that
        /// comment will be made to the Comment node also from that node comment a new connection named Comment will bee made to
        /// the post.
        /// </summary>
        /// <param name="comment"></param>
        /// <returns> If the new comment action was a without error will send "ok" to the client </returns>
        public ResponseEnum CommentOnPost(Comment comment)
        {
            var statment = $"MATCH (p:Post),(u:User)" +
                           $"WHERE p.PostId = \"{comment.postId}\" AND u.Username = \"{comment.CommenterName}\"" +
                           $"CREATE (c:Comment {{Text: \"{comment.Text}\", CommenterName: \"{comment.CommenterName}\", CommentedDate: \"{comment.CommentedDate}\"}})-[:CommentedOn]->(p),(u)-[:Comment]->(c)" +
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

        /// <summary>
        /// When a user wants to block another user he goed to his profile and click block.
        /// then his UserName and the user that he wants to block will be send to the sever,
        /// then from the user node a new "Block" connection will be made to the from you(the user that blocks)
        /// to the user node that his benn blocked
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="blockedUserName"></param>
        /// <returns> If the block user action was a without error will send "ok" to the client </returns>
        public ResponseEnum BlockUser(string userName, string blockedUserName)
        {
            var statment = $"MATCH (u:User),(bu:User)" +
               $"WHERE u.Username = \"{userName}\" AND bu.Username = \"{blockedUserName}\"" +
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

        /// <summary>
        /// When a user wants to Unblock a user he goes to his profile and go to the blocked user list and there e clicks
        /// unblock. then his username and the unblocked user is benn send to the server
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="unBlockedUserName"></param>
        /// <returns> If the unblock user action was a without error will send "ok" to the client </returns>
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

        /// <summary>
        /// When a user wants to follow a user he goes to his profile  clicks
        /// follow. then his username and the follow user is benn send to the server
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="UserToFollow"></param>
        /// <returns> If the follow user action was a without error will send "ok" to the client </returns>
        public ResponseEnum FollowUser(string userName, string UserToFollow)
        {
            var statment = $"MATCH (u:User),(bu:User)" +
               $"WHERE u.Username = \"{userName}\" AND bu.Username = \"{UserToFollow}\"" +
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

        /// <summary>
        /// When a user wants to UnFollow a user he goes to his profile and go to the  user profile and there he clicks
        /// unFollow. then his username and the unblocked user is benn send to the server
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="unFollowUserName"></param>
        /// <returns> If the umfollow user action was a without error will send "ok" to the client </returns>
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

        /// <summary>
        /// When a user goes in his profile and click on show all the blocked users he will come to this function.
        /// from the client the username will be passed to the server. and then every user that as to him the block
        /// connection will be return
        /// </summary>
        /// <param name="userName"></param>
        /// <returns> List of all the blocked users of a specific user </returns>
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
            }
            
            return usertList;
        }

        /// <summary>
        /// When a user goes to a profile and wants to see all the followers of that user he
        /// will come to this function. the client will send the username of that user to the server.
        /// and then every user that as to him the follow
        /// connection will be return
        /// </summary>
        /// <param name="userName"></param>
        /// <returns> List of all the Following users of a specific user </returns>
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
            }
           

            return usertList;
        }

        /// <summary>
        /// When a user wants to se all the comments of a specific post he will come
        /// to this function. the client will send the post id to the server. and then all the nodes 
        /// of comments that as connection to the post will be returend
        /// </summary>
        /// <param name="postId"></param>
        /// <returns> Return all the comments of a specific post</returns>
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
            }
            
            return commentList;
        }
    }
}
