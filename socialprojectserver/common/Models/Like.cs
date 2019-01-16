using SocialProjectServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Like
    {
        public string UserName { get; set; }

        public string postId { get; set; }

        public List<User> UsersWhoLiked { get; set; }

    }
}
