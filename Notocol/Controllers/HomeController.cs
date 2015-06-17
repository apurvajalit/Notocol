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

            if (Session["userID"] != null) return true;

            //if (Request.Cookies["UserInfo"] != null)
            //{
            //    UserRepository userRepo = new UserRepository();
            //    long userID = Convert.ToInt64(Request.Cookies["UserInfo"].Value);
            //    string userName = userRepo.getuserName(userID);
            //    if (userName != null)
            //    {
            //        Session["userID"] = userID;
            //        Session["userName"] = userName;
            //        return true;

            //    }
            //}
            return false;
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
            ViewBag.Title = "Login";

            return View();
        }

        public ActionResult HowItWorks()
        {
            ViewBag.Title = "How it works";
            return View();
        }
        public ActionResult Home()
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



            return View(TagHelper.GetAllUserTags(Convert.ToInt64(Session["userID"])));
        }



        //public ActionResult MyTags()
        //{
        //    IList<Tag> searchTags = SearchMyTags("");
        //    return View(searchTags);
        //}

        //[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        //public JsonResult SearchTags(string id)
        //{
        //    return Json(SearchMyTags(id), JsonRequestBehavior.AllowGet);
        //}

        //public IList<Tag> SearchMyTags(string strSearch)
        //{
        //    TagRepository objTagRepository = new TagRepository();
        //    IList<Tag> searchTags = objTagRepository.SearchTags(strSearch, 2);
        //    return searchTags;
        //}

        //public long AddTag(Tag objTag)
        //{
        //    objTag.UserID = Convert.ToInt64(Session["userID"]);
        //    TagRepository objTagRepository = new TagRepository();
        //    return objTagRepository.SaveTag(objTag).ID;
        //}

        //public bool DeleteTag(Tag objTag)
        //{
        //    objTag.UserID = Convert.ToInt64(Session["userID"]);
        //    TagRepository objTagRepository = new TagRepository();
        //    return objTagRepository.DeleteTag(objTag);
        //}
        /// <summary>
        /// Controller Method to save Source Data
        /// </summary>
        /// <param name="objSource"></param>
        /// <param name="lstTags"></param>
        /// <returns></returns>
        //public long SaveSource(Source objSource, IList<Tag> lstTags)
        //{
            //Source objSourceTemp = new Source();
            //IList<Tag> lstTagsTemp = new List<Tag>();
            //objSourceTemp.Title = "abcd123";
            //objSourceTemp.UserID = 2;

            //Tag objTag = new Tag();
            //objTag.ID = 5;
            //objTag.Name = "Project Management";
            //objTag.ParentID = 1;
            //objTag.UserID = 2;
            //lstTagsTemp.Add(objTag);

            //Tag objTag1 = new Tag();
            //objTag1.ID = 7;
            //objTag1.Name = "Javascript";
            //objTag1.ParentID = 1;
            //objTag1.UserID = 2;
            //lstTagsTemp.Add(objTag1);

            //Tag objTag2 = new Tag();
            //objTag2.ID = 0;
            //objTag2.Name = "Widgets";
            //objTag2.ParentID = 7;
            //objTag2.UserID = 2;
            //lstTagsTemp.Add(objTag2);

           // SourceRepository obSourceRepository = new SourceRepository();
           // obSourceRepository.SaveSource(objSourceTemp, lstTagsTemp);


           // return 0;
        //}

        //public ActionResult SourceItems(string keywordFilter = "", string tagString = "")
        //{
           
           
        //    SourceRepository obSourceRepository = new SourceRepository();
        //    //TODO Enable null check
        //  //  if (Session != null && Session["userID"] != null)
        //        long userID = Convert.ToInt64(Session["userID"]);
            
        //    return PartialView(obSourceRepository.Search(keywordFilter, tagString, userID));
        //}

        
    }
}
