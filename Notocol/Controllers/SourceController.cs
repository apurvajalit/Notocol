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
    public class SourceController : Controller
    {
        // GET: Source
        public ActionResult SourceItems(string keywordFilter = "", string tagFilter = "", int tab = 0)
        {

            long userID = Utility.GetCurrentUserID();
             

            IList<long> tagIDs = new List<long>();
            if (tagFilter != "")
            {
                string[] tags = tagFilter.Split(',');
                tagIDs = (tab == 0) ? TagHelper.GetCurrentUserIDs(userID, tagFilter.Split(',')) : TagHelper.GetAllUserTagIDs(tagFilter.Split(','));
            }
            if(tab == 0)
                return PartialView(new SourceHelper().GetSourceItems(keywordFilter, tagIDs, userID, true));
            else
                return PartialView("SourceItemsWithUser",new SourceHelper().GetSourceItems(keywordFilter, tagIDs, userID, false));
        }

        public bool DeleteSource(long sourceID)
        {
            SourceRepository sourceRepository = new SourceRepository();

            Source source = sourceRepository.GetSource(sourceID);

            if (source != null && source.UserID == Utility.GetCurrentUserID())
            {
                return sourceRepository.DeleteSource(source);

            }
            return false;
        }
    }
}