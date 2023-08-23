using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SmugMug.Net;

namespace SmugMug.Net.Service
{
    public class FriendService
    {
        private Core.SmugMugCore _core;

        public FriendService(Core.SmugMugCore core)
        {
            _core = core;
        }

        /// <summary>
        /// Add a user to a user's list of friends
        /// </summary>
        /// <param name="nickName">The NickName for a specific user</param>
        /// <returns></returns>
        public bool AddFriend(string nickName)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("NickName", nickName);

            _core.QueryWebsite<Data.SmugmugError>("smugmug.friends.add", queryParams, false);

            return true;
        }


        /// <summary>
        /// Retrieve a list of friends for a user
        /// </summary>
        /// <returns></returns>
        public Data.SmugmugFriend[] GetFriendList()
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();

            var queryResponse = _core.QueryWebsite<Data.SmugmugFriend>("smugmug.friends.get", queryParams, true);

            // Return Results
            return queryResponse;
        }

        /// <summary>
        /// Remove a user from a user's list of friends
        /// </summary>
        /// <param name="nickName">The NickName for a specific user</param>
        /// <returns></returns>
        public bool RemoveFriend(string nickName)
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();
            queryParams.Add("NickName", nickName);

            _core.QueryWebsite<Data.SmugmugError>("smugmug.friends.remove", queryParams, false);

            return true;
        }

        /// <summary>
        /// Remove all users from a user's list of friends
        /// </summary>
        /// <returns></returns>
        public bool RemoveAll()
        {
            // Append the parameters from the request object
            var queryParams = new Core.QueryParameterList();

            _core.QueryWebsite<Data.SmugmugError>("smugmug.friends.removeAll", queryParams, false);

            return true;
        }

    }
}
