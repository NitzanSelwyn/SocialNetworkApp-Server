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
        List<Post> GetUserPosts(string userName, int skipNuber);
        List<Post> GetFolowersPosts(string userName, int skipNuber);
        ResponseEnum DeletePost(string postId);
        Post LikePost(Like like);
        Post UnLikePost(Like like);
        Post CommentOnPos(Comment comment);
        //List<string> GetTheUserThatFollowMe(string userName);
        //List<string> GetTheUsersThatIFollow(string userName);
        List<Comment> GetPostsComments(string postId);
        ResponseEnum RegisterUserToNeo4j(string userName, string firstName,string lastName);
        string UploadFile(byte[] file, string authorName);
    }
}
