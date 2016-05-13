using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToTwitter;

namespace DeleteYourTweets.Helpers
{
    public class TwitterHelper
    {
        public static bool IsLogged
        {
            get
            {
                return new SessionStateCredentialStore().HasAllCredentials();
            }
        }

        private static TwitterContext GetContext()
        {
            var auth = new MvcAuthorizer()
            {
                CredentialStore = new SessionStateCredentialStore()
            };
            return new TwitterContext(auth);
        }

        private static User GetMe()
        {
            var context = GetContext();
            return (from user in context.User
                    where user.Type == UserType.Lookup &&
                    user.ScreenNameList == context.Authorizer.CredentialStore.ScreenName
                    select user).SingleOrDefault();
        }

        public static string GetName()
        {
            var user = GetMe();
            return (user != null) ? user.Name : "";
        }

        public static async Task DeleteAllTweets()
        {
            var context = GetContext();
            var me = GetMe();
            string username = context.Authorizer.CredentialStore.ScreenName;
            ulong maxID;
            var statusList = new List<Status>();

            var userStatusResponse =
                (from tweet in context.Status
                 where tweet.Type == StatusType.User &&
                   tweet.ScreenName == username &&
                   tweet.Count == 200
                 select tweet)
                .ToList();
            statusList.AddRange(userStatusResponse);
            if (userStatusResponse.Count > 0)
            {
                maxID = userStatusResponse.Min(
                    status => status.StatusID) - 1;

                do
                {
                    userStatusResponse =
                        (from tweet in context.Status
                         where tweet.Type == StatusType.User &&
                               tweet.ScreenName == username &&
                               tweet.MaxID == maxID &&
                               tweet.Count == 200
                         select tweet)
                        .ToList();

                    if (userStatusResponse.Count > 0)
                    {
                        maxID = userStatusResponse.Min(
                            status => status.StatusID) - 1;

                        statusList.AddRange(userStatusResponse);
                    }
                }
                while (userStatusResponse.Count != 0 && statusList.Count < me.StatusesCount + 1);

                foreach (Status tweet in statusList)
                {
                    await context.DeleteTweetAsync(tweet.StatusID);
                }
            }
        }

        public static async Task UnfollowAllFollowings()
        {
            var context = GetContext();
            var users = new List<User>();
            Friendship friendship;
            long cursor = -1;
            do
            {
                friendship =
                    await
                    (from friend in context.Friendship
                     where friend.Type == FriendshipType.FriendsList &&
                           friend.ScreenName == context.Authorizer.CredentialStore.ScreenName &&
                           friend.Cursor == cursor &&
                           friend.Count == 100
                     select friend)
                    .SingleOrDefaultAsync();

                if (friendship != null &&
                    friendship.Users != null &&
                    friendship.CursorMovement != null)
                {
                    cursor = friendship.CursorMovement.Next;

                    users.AddRange(friendship.Users);
                }

            } while (cursor != 0);
            foreach (User user in users)
            {
                await context.DestroyFriendshipAsync(user.ScreenNameResponse);
            }
        }

        public static async Task DeleteAllSendByMessages()
        {
            var context = GetContext();
            var me = GetMe();
            string username = context.Authorizer.CredentialStore.ScreenName;
            ulong maxID;
            var messageList = new List<DirectMessage>();

            var messagesResponse =
                await (from message in context.DirectMessage
                       where message.Type == DirectMessageType.SentBy &&
                       message.Count == 200
                       select message)
                .ToListAsync();
            messageList.AddRange(messagesResponse);
            if (messagesResponse.Count > 0)
            {
                maxID = messagesResponse.Max(
                    message => message.IDResponse) - 1;

                do
                {
                    messagesResponse =
                       await (from message in context.DirectMessage
                              where
                              message.Type == DirectMessageType.SentBy &&
                              message.MaxID == maxID &&
                              message.Count == 200
                              select message)
                        .ToListAsync();

                    if (messagesResponse.Count > 0)
                    {
                        maxID = messagesResponse.Min(
                            message => message.IDResponse) - 1;

                        messageList.AddRange(messagesResponse);
                    }
                }
                while (messagesResponse.Count != 0);
                foreach (DirectMessage message in messageList)
                {
                        await context.DestroyDirectMessageAsync(message.IDResponse, false);
                }
            }
        }

        public static async Task DeleteAllSendToMessages()
        {
            var context = GetContext();
            var me = GetMe();
            string username = context.Authorizer.CredentialStore.ScreenName;
            ulong maxID;
            var messageList = new List<DirectMessage>();

            var messagesResponse =
                await (from message in context.DirectMessage
                       where message.Type == DirectMessageType.SentTo &&
                       message.Count == 200
                       select message)
                .ToListAsync();
            messageList.AddRange(messagesResponse);
            if (messagesResponse.Count > 0)
            {
                maxID = messagesResponse.Min(
                    message => message.IDResponse) - 1;

                do
                {
                    messagesResponse =
                       await (from message in context.DirectMessage
                              where message.MaxID == maxID &&
                              message.Type == DirectMessageType.SentTo &&
                       message.Count == 200
                              select message)
                        .ToListAsync();

                    if (messagesResponse.Count > 0)
                    {
                        maxID = messagesResponse.Min(
                            message => message.IDResponse) - 1;

                        messageList.AddRange(messagesResponse);
                    }
                }
                while (messagesResponse.Count != 0);
                foreach (DirectMessage message in messageList)
                {
                    await context.DestroyDirectMessageAsync(message.IDResponse, false);
                }
            }
        }
        
        public static async Task DeleteAllFavourites()
        {
            var context = GetContext();
            var me = GetMe();
            ulong maxID;
            var favlist = new List<Favorites>();

            var favsResponse =
                await (from fav in context.Favorites
                       where fav.Type == FavoritesType.Favorites &&
                         fav.ScreenName == me.ScreenNameResponse &&
                         fav.Count == 200
                       select fav)
                .ToListAsync();
            favlist.AddRange(favsResponse);
            if (favsResponse.Count > 0)
            {
                maxID = favsResponse.Min(
                    fav => fav.StatusID) - 1;

                do
                {
                    favsResponse =
                        await (from fav in context.Favorites
                               where fav.Type == FavoritesType.Favorites &&
                                     fav.ScreenName == me.ScreenNameResponse &&
                                     fav.MaxID == maxID &&
                                     fav.Count == 200
                               select fav)
                        .ToListAsync();

                    if (favsResponse.Count > 0)
                    {
                        maxID = favsResponse.Min(
                            fav => fav.StatusID) - 1;

                        favlist.AddRange(favsResponse);
                    }
                }
                while (favsResponse.Count != 0 && favlist.Count < me.FavoritesCount + 1);
                foreach (Favorites fav in favlist)
                {
                    await context.DestroyFavoriteAsync(fav.StatusID);
                }
            }
        }

        public static async Task SendTweet(string tweet)
        {
            var context = GetContext();
            await context.TweetAsync(tweet);
        }
    }
}
