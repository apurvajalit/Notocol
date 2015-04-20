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
         //GET: User
        [HttpPost]
        public ActionResult SignInUser(string userName, string password="", string identifier="")
        {
            long userID = objUserRepository.checkUser(userName, password, identifier);
            if(userID > 0) 
            {
                HttpCookie userInfoCookies = new HttpCookie("UserInfo");
                string token = Convert.ToString(userID);
                userInfoCookies.Value = token;
                
                Response.Cookies.Add(userInfoCookies);

                Session["username"] = userName;
                Session["userID"] = userID;
                return RedirectToAction("Home", "Home");
            }else{
                return View("Error");
            }
            
              
        }

        [HttpPost]
        public ActionResult SignUpUser(string userName, string password="", string identifier="")
        {
            long userID = objUserRepository.addUser(userName, password, identifier);
            if(userID > 0) 
            {
                return SignInUser(userName, password, identifier);
            }else{
                return View("Error");
            }
        }


        public ActionResult SignOutUser(long userID)
        {
            Session.Abandon();

            return View();
        }
        
        [HttpPost]
        public ActionResult DeleteUser(long userID)
        {
            bool deleteStatus = objUserRepository.deleteUser(userID);
            if (deleteStatus)
            {
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

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                Session["Provider"] = result.Provider;
                Session["ProviderUserId"] = result.ProviderUserId;
                Session["username"] = result.UserName;
                return RedirectToAction("Home", "Home");
            }
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