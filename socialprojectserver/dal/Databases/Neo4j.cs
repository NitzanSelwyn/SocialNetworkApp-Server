using Common.Models;
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

        public void UploadPost(Post post)
        {
            var statment = $"MATCH (u:User)" +
                           $"WHERE u.Username = \"{post.Author}\"" +
                           $"CREATE (p:Post {{Author: \"{post.Author}\", Content: \"{post.Content}\", ImageLink: \"{post.ImageLink}\", Likes: \"{post.Like}\"}})" +
                           $"CREATE (u)-[:Posted]->(p)" +
                           $"RETURN *";
            using (var session = _driver.Session())
            {
                var results = session.Run(statment).Consume();
            }
        }

        public List<Post> GetUserPosts(User user)
        {
            List<Post> postList = new List<Post>();

            var statment = $"MATCH (u:User)-[:Posted]->(p:Post)" +
                           $"WHERE u.Username = \"{user.Username}\"" +
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

        public List<Post> GetFolowersPosts(User user)
        {
            List<Post> postList = new List<Post>();

            using (var session = _driver.Session())
            {
                using (var tx = session.BeginTransaction())
                {
                    IStatementResult results = tx.Run(
                        @"MATCH (u:Users)--[:Follow]->(f:Users)
                          WHERE u.UserName == $user.UserName
                          MATCH f --[:Posted]->(p:Posts)
                          RETURN p");
                    foreach (IRecord result in results)
                    {
                        var node = result["p"].As<INode>();
                        var post = node.Properties["PostID"]?.As<Post>();

                        postList.Add(post);
                    }
                    return postList;
                }
            }
        }

        public void DeletePost(Post post)
        {
            using (var session = _driver.Session())
            {
                var greeting = session.ReadTransaction(tx =>
                {
                    var result = tx.Run("CREATE  " +
                                        "SET  ",
                        new { post });
                    return result.Single()[0].As<Post>();
                });
            }

        }
    }
}
