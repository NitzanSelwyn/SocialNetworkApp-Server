using Common.Configs;
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
    public class Neo4jDB
    {
        private readonly IDriver _driver;
        private static readonly object Neo4jLock = new object();

        public Neo4jDB()
        {
            _driver = GraphDatabase.Driver(DatabaseConfigs.neo4jDBConnectionString,
                AuthTokens.Basic(DatabaseConfigs.neo4jDBUserName, DatabaseConfigs.neo4jDBPassword));
        }

        ~Neo4jDB()
        {
            _driver?.Dispose();
        }

        private IStatementResult ExecuteQuery(string query)
        {
            using (var session = _driver.Session())
            {
                return session.Run(query);
            }
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
            var statment = $"MATCH (u:User{{Username: \"{post.Author}\"}})" +
                           $"CREATE (u)-[:Posted]->(p:Post {{Author: \"{post.Author}\", " +
                           $"Content: \"{post.Content}\", ImageLink: \"{post.ImageLink}\", " +
                           $"DatePosted: \"{post.DatePosted}\", PostId: \"{post.PostId}\"}})" +
                           $"RETURN *";


            var statment2 = $"MATCH (p:Post{{PostId: \"{post.PostId}\"}}),(u:User{{Username: \"{post.MantionedUser}\"}})" +
                            $"MERGE (p)-[:Mantioned]->(u)" +
                            $"RETURN u";
            try
            {
                ExecuteQuery(statment);
                ExecuteQuery(statment2);
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
        public List<Post> GetUserPosts(string userName,int skipNuber)
        {
            lock (Neo4jLock)
            {
                List<Post> postList = new List<Post>();

                var statment = $"MATCH (u:User{{Username: \"{userName}\"}})-[:Posted]->(p:Post)" +
                               $"RETURN p ORDER BY p.DatePosted DESC SKIP {skipNuber} LIMIT 10";


                var results = ExecuteQuery(statment);

                var mantanedPosts = GetMantionedPosts(userName);

                if (mantanedPosts != null)
                {
                    foreach (var item in mantanedPosts)
                    {
                        item.MantionedUser = userName;
                        item.FullName = GetUserName(item.Author);
                        postList.Add(item);
                    }
                }

                foreach (var result in results)
                {
                    var nodeProps = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    var post = JsonConvert.DeserializeObject<Post>(nodeProps);
                    post.Like.UsersWhoLiked = GetUsersWhoLikedThePost(post.PostId);
                    post.FullName = GetUserName(post.Author);
                    postList.Add(post);
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
        public List<Post> GetFollowingsPosts(string userName, int skipNuber)
        {
            lock (Neo4jLock)
            {
                List<Post> postList = new List<Post>();

                var statment = $"MATCH (u:User{{Username: \"{userName}\"}})-[:Follow]->(u2:User)-[:Posted]->(p:Post)" +
                               $"WHERE NOT EXISTS ((u)-[:Block]-(u2))" +
                               $"RETURN p ORDER BY p.DatePosted DESC SKIP {skipNuber} LIMIT 10";

                var results = ExecuteQuery(statment);
                var mantanedPosts = GetMantionedPosts(userName);

                if (mantanedPosts != null)
                {
                    foreach (var item in mantanedPosts)
                    {
                        item.MantionedUser = userName;
                        item.FullName = GetUserName(item.Author);
                        postList.Add(item);
                    }
                }

                foreach (var result in results)
                {
                    var nodeProps = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    var post = JsonConvert.DeserializeObject<Post>(nodeProps);
                    post.Like.UsersWhoLiked = GetUsersWhoLikedThePost(post.PostId);
                    post.FullName = GetUserName(post.Author);
                
                    postList.Add(post);
                }

                return postList;

            }
        }

        private List<Post> GetMantionedPosts(string userName)
        {
            lock (Neo4jLock)
            {
                var postList = new List<Post>();

                var statment = $"MATCH (p:Post)-[:Mantioned]->(u:User{{Username: \"{userName}\"}})" +
                               $"RETURN p";

                var results = ExecuteQuery(statment);
                
                foreach (var result in results)
                {
                    var nodeProps = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    var post = JsonConvert.DeserializeObject<Post>(nodeProps);
                    postList.Add(post);
                    return postList;
                }
                return null;
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
            lock (Neo4jLock)
            {
                var statment = $"MATCH (p:Post{{PostId: \"{postId}\"}})" +
                               $"DETACH DELETE p";

                try
                {
                    ExecuteQuery(statment);

                    return ResponseEnum.Succeeded;
                }
                catch (Exception)
                {

                    return ResponseEnum.Failed;
                }
            }
        }

        /// <summary>
        /// When a new user is registering to the website his UserName will be send to this function and a new node will be 
        /// created with his UserName (and if a new users loggin in with facebook his UserName will be also saved)
        /// </summary>
        /// <param name="userName"></param>
        /// <returns> If the RegisterUser was a without error will send "ok" to the client </returns>
        public ResponseEnum RegisterUserToNeo4j(string userName, string firstName, string lastName)
        {
            var statment = $"CREATE (u:User {{Username: \"{userName}\", " +
                $"FirstName: \"{firstName}\", LastName: \"{lastName}\"}})";

            try
            {
                ExecuteQuery(statment);

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
        public Post LikePost(Like like)
        {
            lock (Neo4jLock)
            {

                var statment = $"MATCH (p:Post {{PostId: \"{like.postId}\"}})," +
                               $"(u:User {{Username: \"{like.UserName}\"}})" +
                               $"MERGE (u)-[:Liked]->(p)" +
                               $"RETURN p";
                try
                {

                    var results = ExecuteQuery(statment);

                    foreach (var result in results)
                    {
                        var nodeProps = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                        var post = JsonConvert.DeserializeObject<Post>(nodeProps);
                        post.FullName = GetUserName(post.Author);
                        post.Like.UsersWhoLiked = GetUsersWhoLikedThePost(post.PostId);
                        return post;
                    }
                }
                catch (Exception)
                {
                   // return null;
                }
                return null;
            }
        }

        /// <summary>
        /// When a user as liked a post and wants to unlike a new button will apper on the post
        /// that he likes that allows him to unlike.
        /// by only removing the connection of "Liked" this operation works
        /// </summary>
        /// <param name="like"></param>
        /// <returns> If the Unlike was a without error will send "ok" to the client </returns>
        public Post UnLikePost(Like like)
        {
            lock (Neo4jLock)
            {
                var statment = $"MATCH (u:User{{Username:\"{like.UserName}\"}})-[l:Liked]->(p:Post{{PostId:\"{like.postId}\"}})" +
                               $"DELETE l " +
                               $"RETURN p";

                try
                {

                    var results = ExecuteQuery(statment);

                    foreach (var result in results)
                    {
                        var nodeProps = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                        var post = JsonConvert.DeserializeObject<Post>(nodeProps);
                        post.FullName = GetUserName(post.Author);
                        return post;
                    }
                }
                catch (Exception )
                {
                  //  return null;
                }
                return null;

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
        public Post CommentOnPost(Comment comment)
        {
            lock (Neo4jLock)
            {
                var statment = $"MATCH (p:Post{{PostId: \"{comment.postId}\"}})," +
                               $"(u:User{{Username: \"{comment.CommenterName}\"}})" +
                               $"CREATE (c:Comment {{Text: \"{comment.Text}\", " +
                               $"CommenterName: \"{comment.CommenterName}\", " +
                               $"CommentedDate: \"{comment.CommentedDate}\"}})-[:CommentedOn]->(p)," +
                               $"(u)-[:Comment]->(c)" +
                               $"RETURN p";
                try
                {
                    var results = ExecuteQuery(statment);

                    foreach (var result in results)
                    {
                        var nodeProps = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                        var post = JsonConvert.DeserializeObject<Post>(nodeProps);
                        post.FullName = GetUserName(post.Author);
                        post.Like.UsersWhoLiked = GetUsersWhoLikedThePost(post.PostId);
                        return post;
                    }
                }
                catch (Exception)
                {
                }
                return null;
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
            lock (Neo4jLock)
            {
                var statment = $"MATCH (u:User{{Username: \"{userName}\"}})," +
                               $"(bu:User{{Username: \"{blockedUserName}\"}})" +
                               $"MERGE (u)-[:Block]->(bu)" +
                               $"RETURN *";

                try
                {
                    ExecuteQuery(statment);

                    return ResponseEnum.Succeeded;
                }
                catch (Exception)
                {

                    return ResponseEnum.Failed;
                }
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
            lock (Neo4jLock)
            {
                var statment = $"MATCH (u:User{{Username: \"{userName}\"}})-[b:Block]->(bu:User{{Username: \"{unBlockedUserName}\"}})" +
                               $"DELETE b";

                try
                {
                    ExecuteQuery(statment);

                    return ResponseEnum.Succeeded;
                }
                catch (Exception)
                {

                    return ResponseEnum.Failed;
                }
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
            lock (Neo4jLock)
            {
                var statment = $"MATCH (u:User{{Username: \"{userName}\"}})," +
                               $"(bu:User{{Username: \"{UserToFollow}\"}})" +
                               $"MERGE (u)-[:Follow]->(bu)" +
                               $"RETURN *";

                try
                {
                    ExecuteQuery(statment);

                    return ResponseEnum.Succeeded;
                }
                catch (Exception)
                {

                    return ResponseEnum.Failed;
                }
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
            lock (Neo4jLock)
            {
                var statment = $"MATCH (u:User{{Username: \"{userName}\"}})-[f:Follow]->(bu:User{{Username: \"{unFollowUserName}\"}})" +
                               $"DELETE f";

                try
                {
                    ExecuteQuery(statment);

                    return ResponseEnum.Succeeded;
                }
                catch (Exception)
                {

                    return ResponseEnum.Failed;
                }
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
            lock (Neo4jLock)
            {
                List<string> usertList = new List<string>();

                var statment = $"MATCH (u:User{{Username: \"{userName}\"}})-[:Block]->(bu:User)" +
                               $"RETURN bu";


                var results = ExecuteQuery(statment);

                foreach (var result in results)
                {
                    var nodeProps = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    User user = JsonConvert.DeserializeObject<User>(nodeProps);
                    usertList.Add(user.Username);
                }

                return usertList;
            }
        }

        /// <summary>
        /// When a user goes to a profile and wants to see all the followers of that user he
        /// will come to this function. the client will send the username of that user to the server.
        /// and then every user that as to him the follow
        /// connection will be return
        /// </summary>
        /// <param name="userName"></param>
        /// <returns> List of all the Following users of a specific user </returns>
        public List<string> GetTheUsersThatIFollow(string userName)
        {
            lock (Neo4jLock)
            {
                List<string> usertList = new List<string>();

                var statment = $"MATCH (u:User{{Username: \"{userName}\"}})-[:Follow]->(fu:User)" +
                               $"RETURN fu";


                var results = ExecuteQuery(statment);

                foreach (var result in results)
                {
                    var nodeProps = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    User user = JsonConvert.DeserializeObject<User>(nodeProps);
                    usertList.Add(user.Username);
                }

                return usertList;
            }
        }

        public List<string> GetTheUserThatFollowMe(string userName)
        {
            lock (Neo4jLock)
            {
                List<string> usertList = new List<string>();

                var statment = $"MATCH (u:User)-[:Follow]->(fu:User{{Username: \"{userName}\"}})" +
                               $"RETURN u";


                var results = ExecuteQuery(statment);

                foreach (var result in results)
                {
                    var nodeProps = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    User user = JsonConvert.DeserializeObject<User>(nodeProps);
                    usertList.Add(user.Username);
                }

                return usertList;
            }
        }

        /// <summary>
        /// When a user wants to se all the comments of a specific post he will come
        /// to this function. the client will send the post id to the server. and then all the nodes 
        /// of comments that as connection to the post will be returend
        /// </summary>
        /// <param name="postId"></param>
        /// <returns> Return all the comments of a specific post</returns>
        /// 
        public List<Comment> GetCommentsOfPost(string postId)
        {
            lock (Neo4jLock)
            {
                var commentList = new List<Comment>();

                var statment = $"MATCH (c:Comment)-[:CommentedOn]->(p:Post{{PostId: \"{postId}\"}})" +
                               $"RETURN c";


                var results = ExecuteQuery(statment);

                foreach (var result in results)
                {
                    var nodeProps = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    commentList.Add(JsonConvert.DeserializeObject<Comment>(nodeProps));
                }


                return commentList;
            }
        }

        /// <summary>
        /// When the user request a the posts of his following/enters a profile of a single usser
        /// he recived a List of post so becuse we want to show how many likes and who liked the post
        /// every post id that was found will come to this function and return the username list of that post
        /// </summary>
        /// <param name="postId"></param>
        /// <returns> List of all the usernames who liked a post</returns>
        private List<string> GetUsersWhoLikedThePost(string postId)
        {
            lock (Neo4jLock)
            {
                var usernameList = new List<string>();

                var statment = $"MATCH (u:User)-[:Liked]->(p:Post{{PostId: \"{postId}\"}})" +
                               $"RETURN u";


                var results = ExecuteQuery(statment);

                foreach (var result in results)
                {
                    var nodeProps = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    var user = JsonConvert.DeserializeObject<User>(nodeProps);
                    usernameList.Add(user.Username);
                }

                return usernameList;
            }
        }

        public ResponseEnum UpdateUserDetails(string userName, string firstName, string lastName)
        {
            var statment = $"MERGE (u:User {{Username: \"{userName}\"}})" +
                           $"SET u.FirstName = \"{firstName}\",u.LastName = \"{lastName}\"" +
                           $"RETURN *";

            try
            {
                ExecuteQuery(statment);

                return ResponseEnum.Succeeded;
            }
            catch (Exception)
            {

                return ResponseEnum.Failed;
            }
        }

        private string GetUserName(string userNamne)
        {
            var statment = $"MATCH (u:User{{Username: \"{userNamne}\"}})" +
                           $"WHERE u.Username = \"{userNamne}\"" +
                           $"RETURN u";

            try
            {

                var results = ExecuteQuery(statment);
                foreach (var result in results)
                {
                    var nodeProps = JsonConvert.SerializeObject(result[0].As<INode>().Properties);
                    var user = JsonConvert.DeserializeObject<User>(nodeProps);
                    return $"{user.FirstName} {user.LastName}";
                }


                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
