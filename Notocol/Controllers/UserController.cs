using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Repository;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;

namespace Notocol.Controllers
{
    public class UserController : Controller
    {
        UserRepository objUserRepository = new UserRepository();


        private long CheckLoginUser(string userName, string password, string identifier)
        {
            long userID = objUserRepository.checkUser(userName, password, identifier);
            if (userID > 0)
            {
                LoginUser(userID, userName);
                HttpCookie userInfoCookies = new HttpCookie("UserInfo");
                string token = Convert.ToString(userID);
                userInfoCookies.Value = token;
                userInfoCookies.Expires = DateTime.MaxValue;
                Response.Cookies.Add(userInfoCookies);

                Session["username"] = userName;
                Session["userID"] = userID;

            }
            return userID;
            
        }

        private ActionResult LoginUser(long userID, string userName)
        {
            HttpCookie userInfoCookies = new HttpCookie("UserInfo");
            string token = Convert.ToString(userID);
            userInfoCookies.Value = token;
            userInfoCookies.Expires = DateTime.MaxValue;
            Response.Cookies.Add(userInfoCookies);
            Session["username"] = userName;
            Session["userID"] = userID;
            
            TempData["RefreshExtension"] = true;
            return RedirectToAction("Home", "Home");
        }

        private ActionResult AddNewUser(string userName, string password, string identifier){
            long userID = objUserRepository.addUser(userName, password, identifier);
            if (userID > 0)
            {
                return LoginUser(userID, userName);
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult SignInUser(string userName, string password="", string identifier="")
        {
            if (CheckLoginUser(userName, password, identifier) > 0){
                TempData["RefreshExtension"] = true;
                return RedirectToAction("Home", "Home");
            }
            else return RedirectToAction("Error", "Home");
              
        }

        [HttpPost]
        public ActionResult SignUpUser(string userName, string password="", string identifier="")
        {
            return AddNewUser(userName, password, identifier);
        }


        public ActionResult SignOutUser(long userID=0)
        {
            
            Session.Clear();
            Session.Abandon();
            if (Request.Cookies["UserInfo"] != null)
            {
                Response.Cookies["UserInfo"].Expires = DateTime.Now.AddDays(-1);
            }

            
            return RedirectToAction("Index", "Home", new  {refresh = true }); 
        }
        
        [HttpPost]
        public ActionResult DeleteUser(long userID)
        {
            bool deleteStatus = objUserRepository.deleteUser(userID);
            if (deleteStatus)
            {
                TempData["RefreshExtension"] = true;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            // Rewrite request before it gets passed on to the OAuth Web Security classes
            GooglePlusClient.RewriteRequest();

            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("Index", "Home");
            }
            Session["Provider"] = result.Provider;
            Session["ProviderUserId"] = result.ProviderUserId;
            Session["username"] = result.UserName;

            if (CheckLoginUser(result.UserName, "", result.ProviderUserId) > 0)
            {
                TempData["RefreshExtension"] = true;
                return RedirectToAction("Home", "Home");
            }
            else
            {

                return AddNewUser(result.UserName, "", result.ProviderUserId);
            }
            //User existence check can be implemented here.
            //if (User.Identity.IsAuthenticated)
            //{
            //    // If the current user is logged in add the new account
            //    OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
            //    return RedirectToLocal(returnUrl);
            //}
            //else
            //{
            //    // User is new, ask for their desired membership name
            //    Session["Provider"] = result.Provider;
            //    Session["ProviderUserId"] = result.ProviderUserId;
            //    Session["username"] = result.UserName;
            //    return RedirectToAction("Home", "Home");
            //}
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

    }
}