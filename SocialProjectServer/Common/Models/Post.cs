using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Post
    {

        public string Author { get; set; }

        public string PostId { get; set; }

        public string Content { get; set; }

        public byte[] Image { get; set; }

        public string ImageLink { get; set; }

        public List<Comment> CommentList { get; set; }

        public string FullName { get; set; }

        public DateTime DatePosted { get; set; }

        public Like Like { get; set; }



    }
}
