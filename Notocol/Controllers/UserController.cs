using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Repository;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using Notocol.Models;

namespace Notocol.Controllers
{
    public class UserController : Controller
    {
        UserRepository objUserRepository = new UserRepository();


        private long CheckLoginUser(string userName, string password, string identifier)
        {
            Model.User userDB;
            long userID = objUserRepository.GetAuthorisedUser(userName, password, identifier, out userDB);
            return userID;
        }

        public void SetUserSession(long userID, string userName)
        {
            
            Session["userName"] = userName;
            Session["userID"] = userID;

        }
        public void ResetUserSession()
        {
            Session["userName"] = null;
            Session["userID"] = null;
        }

        private ActionResult AddNewUser(string userName, string password, string email, string identifier){
            Model.User userDB = new Model.User();
            userDB.Username = userName;
            userDB.Password = password;
            userDB.Identifier = identifier;
            userDB.Email = email;
            userDB.ModifiedAt = DateTime.Now;

            if((userDB.ID = objUserRepository.addUser(userDB)) > 0)
            {
                Utility.AddCookie("TOKEN-INFO",Utility.GenerateUserInfoCookieData(userDB.ID, userDB.Username));
                Utility.SetUserSession(userDB.ID, userDB.Username);
                return RedirectToAction("Home", "Home");
            }
            else
            {
                TempData["SignUpFailed"] = true;
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult SignInUser(string userName, string password="", string identifier="")
        {
            long userID = 0;
            if ((userID = CheckLoginUser(userName, password, identifier)) > 0)
            {
                Utility.AddCookie("TOKEN-INFO", Utility.GenerateUserInfoCookieData(userID, userName));
                Utility.SetUserSession(userID, userName);
                TempData["RefreshExtension"] = true;
                return RedirectToAction("Home", "Home");
            }
            else
            {
                TempData["SignInFailed"] = true;
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult SignUpUser(string userName, string password="", string email="", string identifier="")
        {
            return AddNewUser(userName, password, email, identifier);
        }


        public ActionResult SignOutUser()
        {
            Utility.RemoveCookie("TOKEN-INFO");
            Utility.ResetUserSession();
            return RedirectToAction("Index", "Home", new  {refresh = true }); 
        }
        
        [HttpPost]
        public ActionResult DeleteUser(long userID)
        {
            //bool deleteStatus = false;
            //if (deleteStatus)
            //{
            //    TempData["RefreshExtension"] = true;
            //    return RedirectToAction("Index", "Home");
            //}
            //else
            //{
            //    return View("Error");
            //}
            //throw NotImplementedException;
            return View("Error");
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        //}

        ////
        //// GET: /Account/ExternalLoginCallback

        //[AllowAnonymous]
        //public ActionResult ExternalLoginCallback(string returnUrl)
        //{
        //    // Rewrite request before it gets passed on to the OAuth Web Security classes
        //    GooglePlusClient.RewriteRequest();

        //    AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        //    if (!result.IsSuccessful)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    Session["Provider"] = result.Provider;
        //    Session["ProviderUserId"] = result.ProviderUserId;
        //    Session["username"] = result.UserName;

        //    if (CheckLoginUser(result.UserName, "", result.ProviderUserId) > 0)
        //    {
        //        TempData["RefreshExtension"] = true;
        //        return RedirectToAction("Home", "Home");
        //    }
        //    else
        //    {

        //        return AddNewUser(result.UserName, "", result.ProviderUserId);
        //    }
        //    //User existence check can be implemented here.
        //    //if (User.Identity.IsAuthenticated)
        //    //{
        //    //    // If the current user is logged in add the new account
        //    //    OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
        //    //    return RedirectToLocal(returnUrl);
        //    //}
        //    //else
        //    //{
        //    //    // User is new, ask for their desired membership name
        //    //    Session["Provider"] = result.Provider;
        //    //    Session["ProviderUserId"] = result.ProviderUserId;
        //    //    Session["username"] = result.UserName;
        //    //    return RedirectToAction("Home", "Home");
        //    //}
        //}

        //private ActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

        //internal class ExternalLoginResult : ActionResult
        //{
        //    public ExternalLoginResult(string provider, string returnUrl)
        //    {
        //        Provider = provider;
        //        ReturnUrl = returnUrl;
        //    }

        //    public string Provider { get; private set; }
        //    public string ReturnUrl { get; private set; }

        //    public override void ExecuteResult(ControllerContext context)
        //    {
        //        OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
        //    }
        //}

    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  