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

            var statment = $"MATCH (u:User)-[:Follow]->(u2:User)" +
                           $"WHERE u.Username = \"{userName}\"" +
                           $"MATCH (u2)-[:Post]->(p:Post)" +
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

        public void DeletePost(Post post)
        {
            var statment = $"MATCH (p:Post {{Author: \"{post.Author}\"}}) DETACH DELETE p";

            using (var session = _driver.Session())
            {
                var results = session.Run(statment);
            }
        }
    }
}
