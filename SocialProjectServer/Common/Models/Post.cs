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

        public string Content { get; set; }

        //public List<Comment> Comments { get; set; }

        public string ImageLink { get; set; }

        public int Like { get; set; }
    }
}
