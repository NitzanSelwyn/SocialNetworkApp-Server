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

        public byte[] Image { get; set; }

        public string ImageLink { get; set; }
    }
}
