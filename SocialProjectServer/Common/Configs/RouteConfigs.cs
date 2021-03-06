﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Configs
{
    public static class RouteConfigs
    {
        //Notifications
        public const string PassNotificationToServiceRoute = "Api/Notification/PassNotification";

        //AUTH AND TOKENS
        public const string GetUserIdByTokenRoute = "Api/Auth/GetUsernameByToken";
        public const string GetTokenInsideRoute = "Api/Auth/GetInsideToken";
        public const string ValidateTokenInsideRoute = "Api/Auth/ValidateInsideToken";
        public const string GetTokenRoute = "Api/Users/GetToken";
        public const string ValidateTokenRoute = "Api/Users/ValidateToken";

        //POSTS
        public const string PostNewMessage = "Api/Post";
        public const string GetUsersPosts = "Api/Post/UsersPosts";
        public const string GetFolowersPosts = "Api/Post/FollowingPosts";
        public const string EditPost = "Api/Post/edit";
        public const string DeletePost = "Api/Post";
        public const string Like = "Api/Like";
        public const string UnLike = "Api/UnLike";
        public const string GetUsersWhoLiked = "Api/UsersWhoLiked";
        public const string CommentOnPost = "Api/Comment";
        public const string GetPostsComments = "Api/GetPostsComments";

        //USERS
        public const string GetMyUserRoute = "Api/users/GetUserByToken";
        public const string UserLoginRoute = "Api/Users/UserLogin";
        public const string UserRegisterRoute = "Api/Users/UserRegister";
        public const string UsernameExistsRoute = "Api/Users/UsernameExists";
        public const string EditUserDetailsRoute = "Api/Users/EditUserDetails";
        public const string SearchUsersRoute = "Api/Users/SearchForUsers";
        public const string BlockedByUsersRoute = "Api/Users/BlockedByUser";
        public const string GetUserByUsername = "Api/Users/GetUserByUsername";
        public const string GetTheUsersThatIFollow = "Api/GetTheUsersThatIFollow";
        public const string GetTheUserThatFollowMe = "Api/GetTheUserThatFollowMe";
        public const string FacebookLoginRoute = "Api/Users/FacebookLogin";

        public const string ClearNotificationsRoute = "Api/User/ClearNotifications";
        public const string ClearNotificationsInsideRoute = "Api/Notification/ClearNotifications";
        public const string GetNotificationCount = "Api/Users/GetNotificationCount";
        public const string GetNotificationCountInsideRoute = "Api/Notification/GetNotificationCount";
        public const string GetNotifications = "Api/Users/GetNotifications";
        public const string GetNotificationsInsideRoute = "Api/Notification/GetNotifications";

        //SETTINGS
        public const string ManageRequestRoute = "Api/Settings/ManageRequest";
        public const string GetBlockedUsers = "Api/Settings/GetBlockedUsers";
        public const string GetFollowingUsers = "Api/Settings/GetFollowingUsers";
        public const string GetUsersThatFollowsMe = "Api/Settings/GetUsersThatFollowsMe";
        public const string EditUserPasswordRoute = "Api/Settings/ChangePassword";
    }
}
