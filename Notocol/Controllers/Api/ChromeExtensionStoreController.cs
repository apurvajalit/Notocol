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
                addCookie("XSRF-TOKEN", newCookieToken);
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
        private void addCookie(string name, string value)
        {
            var resp = new HttpResponseMessage();

            var cookie = new HttpCookie(name, value);
            cookie.Expires = DateTime.Now.AddDays(30);
            cookie.Domain = null;
            cookie.Path = "/";

            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        private HttpResponseMessage appDisableUser(ExtensionUser user)
        {
            UserRepository objUserRepository = new UserRepository();
            Model.User userDB = objUserRepository.GetExistingUser(user.Username, user.pwd);
            if (userDB == null || !objUserRepository.deleteUser(userDB))
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
                Model.User userDB = objUserRepository.GetExistingUser(user.Username, oldPassword);
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

            ResetUserSession();
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
                long ret = objUserRepository.checkUser(user.Username, user.Password, null, out userDB);
                if (ret <= 0)
                {

                    ApplicationStatus appErrorStatus = GetAppResponseWithTokens();
                    appErrorStatus.status = "failure";
                    appErrorStatus.errors = new ApplicationErrors();

                    if (ret == 0)
                        appErrorStatus.errors.username = "User does not exist";
                    else if (ret == -1)
                        appErrorStatus.errors.username = "Incorrect password. Please try again";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, appErrorStatus);
                }
                user.ID = ret;
            }

            SetUserSession(user.ID, user.Username);

            return Request.CreateResponse(HttpStatusCode.OK, GetAppResponseWithTokens(user.Username));
        }
        private HttpResponseMessage appRegister(ExtensionUser user)
        {
            //user.Username = "acct:" + user.Username + "@hypothes.is";
            UserRepository objUserRepository = new UserRepository();

            if ((user.ID = objUserRepository.addUser(Utility.ExtensionUserToUser(user))) > 0)
            {
                return appLogIn(user);
            }
            else
            {
                HttpError err = new HttpError("Could not add user");
                return Request.CreateResponse(HttpStatusCode.InternalServerError, GetAppResponseWithTokens());
            }

        }
                
        [HttpGet]
        public HttpResponseMessage appStatus(string __formid__=null)//Possible types: status, profile 
        {
            ApplicationStatus applicationStatus = null;
            string userName = Utility.GetCurrentUserName();

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

        [HttpPost]
        public HttpResponseMessage accountAction(string __formid__, ExtensionUser user)//Possible actions: login, logout
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
