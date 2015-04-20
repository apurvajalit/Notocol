using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Repository;
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
    }
}