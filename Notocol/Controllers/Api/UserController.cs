using Business;
using Notocol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;



namespace Notocol.Controllers.Api
{
    public class UserController : ApiController
    {
        // GET: User
        [HttpGet]
        public bool IsUserLoggedIn()
        {
            if (Utility.GetCurrentUserID() <= 0)
            {
                CookieHeaderValue tokenInfoCookie = null;
                

                tokenInfoCookie =  Request.Headers.GetCookies
                                    (UserHelper.USER_INFO_TOKEN).SingleOrDefault();

                string data = (tokenInfoCookie != null) ? tokenInfoCookie[UserHelper.USER_INFO_TOKEN].Value : null;
                //Check if tokenInfo cookie exists, if found set session with values found in that

                if (data != null)
                {
                    long userID = 0;
                    string userName = null;
                    if (Utility.GetUserInfoFromCookieData(data, out userID, out userName))
                    {
                        Utility.SetUserSession(userID, userName);
                        return true;
                    }
                }

                return false;
            }
            return true;
        }

        [HttpPost]
        public bool AddFollower(long follower, long followee)
        {
            return new UserHelper().AddFollower(follower, followee);
        }

        [HttpDelete]
        public bool DeleteFollower(long follower, long followee)
        {
            return new UserHelper().DeleteFollower(follower, followee);
        }
    }
}