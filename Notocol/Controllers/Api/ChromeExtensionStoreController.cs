using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using AutoMapper.Mappers;
using Model.Extended.Extension;
using AutoMapper;
using Model;
using Repository;
using Notocol.Models;

namespace h_store.Controllers.Api
{
      
    public class ChromeExtensionStoreController : ApiController
    {
        
        private bool CheckXSRFToken()
        {
            string cookieToken = null, tokenFromHeader = null;
            CookieHeaderValue clientCookie = null;
            clientCookie = Request.Headers.GetCookies
                                    ("XSRF-TOKEN").SingleOrDefault();

            cookieToken = (clientCookie != null) ? clientCookie["XSRF-TOKEN"].Value : null;
            IEnumerable<string> headerTokenString = null;
            if(Request.Headers.TryGetValues("X-CSRF-Token", out headerTokenString)){
                tokenFromHeader = headerTokenString.FirstOrDefault<string>();
            }


            try
            {
                AntiForgery.Validate(cookieToken, tokenFromHeader);
            }
            catch
            {
                return false;
            }
            return true;
        }
        private ApplicationStatus GetAppResponseWithTokens(string userName = null)
        {
            ApplicationStatus applicationStatus = new ApplicationStatus();
            applicationStatus.model = new UserApplicationData();
            applicationStatus.flash = new FlashData();
            applicationStatus.status = "okay";
            string newCookieToken = null, formToken = null;

            CookieHeaderValue clientCookie = null;
            clientCookie = Request.Headers.GetCookies
                                    ("XSRF-TOKEN").SingleOrDefault();

            string oldCookieToken = (clientCookie != null) ? clientCookie["XSRF-TOKEN"].Value : null;


            AntiForgery.GetTokens(oldCookieToken, out newCookieToken, out formToken);

            if (newCookieToken != null)
            {
                Utility.AddCookie("XSRF-TOKEN", newCookieToken);
                System.Web.HttpContext.Current.Session["XSRF-TOKEN"] = newCookieToken;
      
            }
            
            System.Web.HttpContext.Current.Session["FORM-XSRF-TOKEN"] = formToken;
            applicationStatus.model.csrf = formToken;
            applicationStatus.model.userid = userName;
            return applicationStatus;
        }
        private void SetUserSession(long userID, string userName)
        {
            System.Web.HttpContext.Current.Session["userID"] = userID;
            System.Web.HttpContext.Current.Session["userName"] = userName;
           // System.Web.HttpContext.Current.Session["userID"] = user.Id;
        }
        private void ResetUserSession()
        {
            System.Web.HttpContext.Current.Session["userID"] = null;
            System.Web.HttpContext.Current.Session["userName"] = null;

            //System.Web.HttpContext.Current.Session["userID"] = null;
        }
        
        private HttpResponseMessage appDisableUser(ExtensionUser user)
        {
            UserRepository objUserRepository = new UserRepository();
            Model.User userDB = objUserRepository.GetOwnRegisteredUser(user.Username, user.pwd);
            if (userDB == null || !objUserRepository.DeleteUser(userDB))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, GetAppResponseWithTokens());
            }

            return Request.CreateResponse(HttpStatusCode.OK, GetAppResponseWithTokens());
        }
        private HttpResponseMessage appEditProfile(ExtensionUser user)
        {
            UserRepository objUserRepository = new UserRepository();


            HttpError err = null;

            if (user.pwd != null)
            {
                //Password Change
                string oldPassword = user.pwd;
                string newPassword = user.Password;
                Model.User userDB = objUserRepository.GetOwnRegisteredUser(user.Username, oldPassword);
                //user.Username = "acct:" + user.Username + "@hypothes.is";
                long current_ID = Utility.GetCurrentUserID();
                if (userDB != null && userDB.ID == current_ID)
                {
                    if (!(objUserRepository.ChangePassword(userDB, newPassword)))
                    {
                        err = new HttpError("Could not change password");
                    }
                }
                else
                {
                    err = new HttpError("Invalid User");
                }
            }
            //TODO Add handling to edit profile: change in subscriptions


            if (err != null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, GetAppResponseWithTokens());

            return Request.CreateResponse(HttpStatusCode.OK, GetAppResponseWithTokens(user.Username));
        }
        private HttpResponseMessage appForgotPassword(ExtensionUser user)
        {

            //TODO Add complete handling and flash message that email has been sent
            return Request.CreateResponse(HttpStatusCode.OK, GetAppResponseWithTokens());

        }
        private HttpResponseMessage appLogOut()
        {

            //System.Web.HttpContext.Current.Session.Abandon();
            //Invalidate the existing token
            //TODO Set flash message to notify user that they have logged out

            Utility.ResetUserSession();
            Utility.RemoveCookie("TOKEN-INFO");
            ApplicationStatus response = GetAppResponseWithTokens();
            response.flash.success = new string[1] { "You have been successfully logged out" };
            return Request.CreateResponse(HttpStatusCode.OK, response);



        }
        private HttpResponseMessage appLogIn(ExtensionUser user)
        {
            UserRepository objUserRepository = new UserRepository();
            if (user.ID <= 0)
            {
                Model.User userDB = null;
                // user.username = "acct:" + user.username + "@hypothes.is";
                //Request has come from the application
                long ret = objUserRepository.AuthenticateOwnUser(user.Username, user.Password, out userDB);
                if (ret != UserRepository.AUTH_USER_AUTHENTICATED){

                    ApplicationStatus appErrorStatus = GetAppResponseWithTokens();
                    appErrorStatus.status = "failure";
                    appErrorStatus.errors = new ApplicationErrors();

                    if (ret == UserRepository.AUTH_USER_NOT_FOUND)
                        appErrorStatus.errors.username = "User does not exist";
                    else if (ret == UserRepository.AUTH_USER_PASSWORD_INCORRECT)
                        appErrorStatus.errors.username = "Incorrect password. Please try again";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, appErrorStatus);
                }
                user.ID = ret;
            }
            Utility.AddCookie("TOKEN-INFO", Utility.GenerateUserInfoCookieData(user.ID, user.Username));
            Utility.SetUserSession(user.ID, user.Username);

            return Request.CreateResponse(HttpStatusCode.OK, GetAppResponseWithTokens(user.Username));
        }
        private HttpResponseMessage appRegister(ExtensionUser user)
        {
            //user.Username = "acct:" + user.Username + "@hypothes.is";
            UserRepository objUserRepository = new UserRepository();
            User userDB = Utility.ExtensionUserToUser(user);
            if ((user.ID = objUserRepository.AddUser(ref userDB)) > 0)
            {
                return appLogIn(user);
            }
            else
            {
                ApplicationStatus status = GetAppResponseWithTokens();
                status.status = "failure";
                status.errors = new ApplicationErrors();
                status.errors.username = "Username already exists, try another";

                return Request.CreateResponse(HttpStatusCode.BadRequest, status);
            }

        }
                
        [HttpGet]
        public HttpResponseMessage app(string __formid__=null)//Possible types: status, profile 
        {
            ApplicationStatus applicationStatus = null;
            string userName = Utility.GetCurrentUserName();
            if (userName == null || userName == "")
            {
                CookieHeaderValue tokenInfoCookie = null;
                tokenInfoCookie = Request.Headers.GetCookies
                                    ("TOKEN-INFO").SingleOrDefault();

                string data = (tokenInfoCookie != null) ? tokenInfoCookie["TOKEN-INFO"].Value : null;
                //Check if tokenInfo cookie exists, if found set session with values found in that

                if (data != null)
                {
                    long userID = 0;
                    if (Utility.GetUserInfoFromCookieData(data, out userID, out userName))
                    {
                        Utility.SetUserSession(userID, userName);
                    }
                }

            }
            applicationStatus = GetAppResponseWithTokens(userName);
            
            
            if (__formid__ == null) 
                return Request.CreateResponse(HttpStatusCode.OK, applicationStatus);
            
            else if (__formid__ == "profile"){
                //TODO Fill profile data from data in DB
                if (userName == null){
                    HttpError err = new HttpError("User not logged in");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, err);
                }

                //TODO Take care of the following
                //UserSubsciptions subscription = new UserSubsciptions();
                //subscription.active = true;
                //subscription.id = user.ID;
                //subscription.type = "reply";
                //subscription.uri = user.Username;

                //applicationStatus.subscriptions = new UserSubsciptions[1] { subscription };

               return Request.CreateResponse(HttpStatusCode.OK, applicationStatus);
            }
                
            throw new NotImplementedException();
        }


        [HttpGet]
        public HttpResponseMessage Features()
        {
            Features features = new Features();
            features.claim = true;
            features.notification = true;
            features.queue = true;
            features.streamer = true;
            features.groups = false;
            features.search_normalized = false;


            return Request.CreateResponse(HttpStatusCode.OK, features);

        }
        [HttpPost]
        public HttpResponseMessage app(string __formid__, ExtensionUser user)//Possible actions: login, logout
        {
            if (!CheckXSRFToken())
            {
                HttpError err = new HttpError("Could not validate the request");
                return Request.CreateResponse(HttpStatusCode.BadRequest, err);
            }

            if (__formid__ == "login")
                return appLogIn(user);
            if (__formid__ == "logout")
                return appLogOut();
            if (__formid__ == "forgot_password")
                return appForgotPassword(user);
            if (__formid__ == "register")
                return appRegister(user);
            if (__formid__ == "edit_profile")
                return appEditProfile(user);
            if (__formid__ == "disable_user")
                return appDisableUser(user);
            if (__formid__ == "reset_password")
                return appDisableUser(user);

            throw new NotImplementedException();
    
        }
    }

}
