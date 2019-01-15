using Common.Enums;
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
        ResponseEnum AddNewPost(Post post);
        List<Post> GetUserPosts(string userName);
        List<Post> GetFolowersPosts(string userName);
        ResponseEnum DeletePost(string postId);
        ResponseEnum LikePost(Like like);
        ResponseEnum CommentOnPos(Comment comment);
        List<Comment> GetPostsComments(string postId);
        ResponseEnum RegisterUserToNeo4j(string userName);
        string UploadFile(byte[] file, string authorName);
    }
}
