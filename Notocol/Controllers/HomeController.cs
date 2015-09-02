using System.Collections.Generic;
using System.Web.Mvc;
using Model;
using Notocol.Controllers.Api;
using System;
using Notocol.Models;
using System.Web;

namespace Notocol.Controllers
{
    public class HomeController : BaseController
    {
        //long userID = 0;
        private bool GetUserFromCookie()
        {
            string cookieData;

            if (Request.Cookies["TOKEN-INFO"] != null)
            {
                string userName = null;
                long userID = 0;
                if ((cookieData = Request.Cookies["TOKEN-INFO"].Value) != null &&
                    Utility.GetUserInfoFromCookieData(HttpUtility.UrlDecode(cookieData), out userID, out userName))
                {
                    Utility.SetUserSession(userID, userName);
                    return true;
                }
            }
            return false;
        }
        private bool checkSession(){

            if (Utility.GetCurrentUserID() != 0  || GetUserFromCookie()) return true;

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

        class apiData
        {
            public string token;
            public string secret;
        };
        
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

        public ActionResult DownloadChromeExtension()
        {
            string filename = "chrome.crx";
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "/Content/DownloadFiles/" + filename;
            byte[] filedata = System.IO.File.ReadAllBytes(filepath);
            string contentType = MimeMapping.GetMimeMapping(filepath);

            System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
        }
    }
}
