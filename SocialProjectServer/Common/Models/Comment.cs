using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Comment
    {
        public string CommenterName { get; set; }
        public DateTime CommentedDate { get; set; }
        public string Text { get; set; }
        public string postId { get; set; }

    }
}
