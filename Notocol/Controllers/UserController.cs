using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using Notocol.Models;
using Model;
using Business;
using Repository;
using Model.Extended;
using System.Net.Http;
using System.Net;
using System.IO;
using Newtonsoft.Json;
namespace Notocol.Controllers
{
    //[SessionState(System.Web.SessionState.SessionStateBehavior.Required)]
    public class UserController : Controller
    {
        UserHelper userHelper = new UserHelper();

        private void SetupForLogin(User user)
        {
            Utility.AddCookie("TOKEN-INFO", Utility.GenerateUserInfoCookieData(user.ID, user.Username));

            Utility.SetUserSession(user.ID, user.Username);
            TempData["RefreshExtension"] = true;
        }

        private void SetupForLogout()
        {
            Utility.RemoveCookie("TOKEN-INFO");
            Utility.ResetUserSession();
            TempData["RefreshExtension"] = true;
        }

        private void SetUserSession(long userID, string userName)
        {

            Session["userName"] = userName;
            Session["userID"] = userID;

        }
        private void ResetUserSession()
        {
            Session["userName"] = null;
            Session["userID"] = null;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (TempData["SignInFailed"] != null)
                ViewBag.SignInError = true;

            if (TempData["SignUpFailed"] != null)
                ViewBag.SignUpError = true;
            
            if (TempData["UserNotFound"] != null)
                ViewBag.UserNotFound = true;

            if (TempData["IncorrectPassword"] != null)
                ViewBag.IncorrectPassword = true;

            if (TempData["NameExists"] != null)
                ViewBag.NameExists = true;

            if (TempData["EmailExists"] != null)
                ViewBag.EmailExists = true;


            
            ViewBag.Title = "Login";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult SignOutUser()
        {

            SetupForLogout();
            return RedirectToAction("Index", "Home", new { refresh = true });
        }
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            // Rewrite request before it gets passed on to the OAuth Web Security classes
            GooglePlusClient.RewriteRequest();

            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));

            if (!result.IsSuccessful)
            {
                ViewBag.SignInError = true;
                return RedirectToAction("Login", "User");
            }
            
            User user = userHelper.GetExternalUser(result.Provider, result.ProviderUserId);

            if (user != null)
            {
                SetupForLogin(user);
                return RedirectToAction("Home", "Home");
            }else{
                Session["AccessToken"] = result.ExtraData["accesstoken"];
                user = new User();
                user.Identifier = result.ProviderUserId;
                user.Provider = result.Provider;
                user.Name = ViewBag.ExternalUserName= result.ExtraData["name"];
                if (user.Provider == "GooglePlus")
                {
                    user.Email = result.ExtraData.ContainsKey("email")? result.ExtraData["email"]:null;
                    user.Gender = result.ExtraData.ContainsKey("gender")?
                                    ((result.ExtraData["gender"] != null) ? 
                                        (result.ExtraData["gender"] == "female" ? "F" : "M")
                                    :null) 
                                  : null;

                    user.Photo = result.ExtraData.ContainsKey("picture")?result.ExtraData["picture"]:null;
                }
                Session["ExternalUser"] = user;
                ViewBag.Title = "Welcome " + user.Name;
                return View("ExternalUsername");
            }
        }

        public ActionResult RegisterExternalUser(string username)
        {

            if(Session["ExternalUser"] == null || Session["AccessToken"] == null)
                
            {
                ViewBag.SignInError = true;
                return RedirectToAction("Login", "User");
            }
            //Check User Name
            if (userHelper.CheckIfUserNameExists(username))
            {
                ViewBag.UsernameExists = true;
                return View("ExternalUsername");
            }

            User user = new User();
            User sessionUser = (User) Session["ExternalUser"];
            
            string accessToken = null, uri = null;
            accessToken =  Session["AccessToken"].ToString();

            if ( user.Provider == "facebook")
            {   
                uri = "https://graph.facebook.com/v2.4/me?access_token=" + accessToken + "&fields=id%2Cname%2Cpicture%7Burl%7D%2Cgender%2Cemail%2Cis_verified&format=json&method=get&pretty=0&suppress_http_code=1";

                StreamReader streamReader = null;
                try
                {
                    using (var client = new HttpClient())
                    {
                        Uri myUri = new Uri(uri);
                        // Create a new request to the above mentioned URL.	
                        WebRequest myWebRequest = WebRequest.Create(myUri);
                        // Assign the response object of 'WebRequest' to a 'WebResponse' variable.
                        WebResponse myWebResponse = myWebRequest.GetResponse();
                        streamReader = new StreamReader(myWebResponse.GetResponseStream());


                    }
                }
                catch (WebException e)
                {
                    ViewBag.SignInError = true;
                    return RedirectToAction("Login", "User");
                }
                dynamic response = JsonConvert.DeserializeObject(streamReader.ReadToEnd());
                user.Name = response["name"] != null ? response["name"] : null;
                user.Email = response["email"] != null ? response["email"] : null;
                user.Gender = response["gender"] != null ? (response["gender"] == "female" ? "F" : "M") : null;
                user.Photo = response["picture"] != null ?
                    (response["picture"]["data"]["url"] != null ? response["picture"]["data"]["url"] : null) : null;


            }else if (sessionUser.Provider == "GooglePlus"){
                user = sessionUser;
            }
            user.Username = username;
            user.Provider = sessionUser.Provider;
            user.Identifier = sessionUser.Identifier;

            Session["ExternalUser"] = null;        
            long retValue = userHelper.AddExternalNewUser(ref user);
            if (retValue > 0)
            {
                SetupForLogin(user);
                return RedirectToAction("Home", "Home");
            }
            ViewBag.SignUpError = true;
            return View("Login");
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        [HttpPost]
        public ActionResult SignInUser(string userName, string password)
        {
            User user = null;
            long authRet = 0;
            if ((authRet = userHelper.AuthenticateOwnUser(userName, password, out user)) == UserRepository.AUTH_USER_AUTHENTICATED)
            {
                SetupForLogin(user);
                return RedirectToAction("Home", "Home");
            }
            else
            {
                TempData["SignInFailed"] = true;

                switch (authRet)
                {
                    case UserRepository.AUTH_USER_NOT_FOUND:
                        TempData["UserNotFound"] = true;
                        break;

                    case UserRepository.AUTH_USER_PASSWORD_INCORRECT:
                        TempData["IncorrectPassword"] = true;
                        break;

                }
                return RedirectToAction("Login", "User");
            }
        }

        [HttpPost]
        public ActionResult SignUpUser(UserRegistration userRegistration)
        {
            User user = null;
            long addStatus = userHelper.AddOwnNewUser(userRegistration, out user);
            if (addStatus > 0)
            {
                SetupForLogin(user);
                return RedirectToAction("Home", "Home");
            }

            TempData["SignUpFailed"] = true;
            switch (addStatus)
            {
                case UserRepository.ADD_USER_NAME_EXISTS:
                    TempData["NameExists"] = true;
                    break;
                case UserRepository.ADD_USER_EMAIL_EXISTS:
                    TempData["EmailExists"] = true;
                    break;
            }
            return RedirectToAction("Login", "User");
        }

        //[HttpPost]
        //public ActionResult DeleteUser(long userID)
        //{
        //    bool deleteStatus = false;
        //    if (deleteStatus)
        //    {
        //        TempData["RefreshExtension"] = true;
        //        return RedirectToAction("Index", "Home");
        //    }
        //    else
        //    {
        //        return View("Error");
        //    }
        //    throw NotImplementedException;
        //    return View("Error");
        //}
    }
}