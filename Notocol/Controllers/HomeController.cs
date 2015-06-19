using System.Collections.Generic;
using System.Web.Mvc;
using Model;
using Notocol.Controllers.Api;
using System;
using Notocol.Models;

namespace Notocol.Controllers
{
    public class HomeController : BaseController
    {
        //long userID = 0;

        private bool checkSession(){

            if (Session["userID"] != null && Convert.ToInt64(Session["userID"]) != 0) return true;

            return false;
        }

        public ActionResult Welcome()
        {
            return View();
        }
        public ActionResult Index(bool refresh = false)
        {
            Debug("Home Loaded");
            //throw (new Exception("File not found"));
            ViewBag.Title = "Home Page";
           // SaveSource(null, null);
            

            if(checkSession()){
                return RedirectToAction("Home");
            }

            if (refresh)
            {
                ViewBag.RefreshExtension = 1;
            }
            else
                ViewBag.RefreshExtension = 0;

            return View();
            
           // return Redirect("default.htm");

        }
        public ActionResult TestPopup()
        {
            ViewBag.Title = "Testing the working of our extension UI";
            
            return View();

        }

        public ActionResult AboutUs()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Login()
        {
            if (TempData["SignInFailed"] != null)
                ViewBag.SignInError = true;
            
            if (TempData["SignUpFailed"] != null)
                ViewBag.SignUpError = true;
            ViewBag.Title = "Login";

            return View();
        }

        public ActionResult HowItWorks()
        {
            ViewBag.Title = "How it works";
            return View();
        }
        public ActionResult Home(string queryFilter = "", string tagFilter = "")
        {
            if (!checkSession())
            {
                RedirectToAction("Index");
            }
            
            
            long userID = Convert.ToInt64(Session["userID"]);
            if (TempData["RefreshExtension"] != null)
            {
                ViewBag.RefreshExtension = 1;
                
            }else
                ViewBag.RefreshExtension = 0;


            ViewBag.QueryFilter = queryFilter;
            ViewBag.TagFilter = tagFilter;
            return View();
        }
    }
}
