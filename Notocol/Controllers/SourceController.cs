using Notocol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Notocol.Controllers
{
    public class SourceController : Controller
    {
        // GET: Source
        public ActionResult SourceItems(string keywordFilter = "", string tagFilter = "")
        {

            long userID = Utility.GetCurrentUserID();
            //string[] tags = { "abc", "def" };
            

            long[] tagIDs = { };
            if (tagFilter != "")
            {
                string[] tags = tagFilter.Split(','); 
                tagIDs = Array.ConvertAll(tags, s => long.Parse(s));
            }
            return PartialView(SourceHelper.GetSourceItems(keywordFilter, tagIDs, userID));
        }
    }
}