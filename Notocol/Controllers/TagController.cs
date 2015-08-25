using Model;
using Notocol.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;

namespace Notocol.Controllers
{
    public class TagController : Controller
    {
        // GET: Tag
        //public JsonResult GetCurrentUserTagNames(string q = "")
        //{

        //    long userID = Utility.GetCurrentUserID(); 
        //    return Json(TagHelper.GetCurrentUserTagNames(userID, q), JsonRequestBehavior.AllowGet);

        //}

        public JsonResult GetAllUserTags(string q = "")
        {

           
            return Json(TagHelper.GetAllUserTag(q), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetCurrentUserTags(string q = "")
        {
            
            return Json(TagHelper.GetCurrentUserTag(Utility.GetCurrentUserID(),q), JsonRequestBehavior.AllowGet);
            

        }
    }
}