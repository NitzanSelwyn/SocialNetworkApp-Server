using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Contracts.Managers
{
    public interface IPostManager
    {
        void AddNewPost(Post post);
        List<Post> GetUserPosts(string userName);
        List<Post> GetFolowersPosts(string userName);
        void DeletePost(string postId);
        void LikePost(Like like);
        void CommentOnPos(Comment comment);
        List<Comment> GetPostsComments(string postId);
        string UploadFile(byte[] file, string authorName);
    }
}
