using Model;
using Notocol.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;
using Model.Extended;
using Repository.Search;

namespace Notocol.Controllers
{
    public class SourceController : Controller
    {
        // GET: Source
        public ActionResult SourceItems(string query = null, SourceFilter filter = null, bool onlySelf = false)
        {

            long userID = Utility.GetCurrentUserID();
            List<string> tags = null;
            if (filter != null) tags = filter.tags;
            ElasticSearchTest es = new ElasticSearchTest();
            if(query == null){
                return PartialView("ESSource", es.PopulateDefaultFeed(userID, onlySelf));
            }else{
                return PartialView("ESSource", es.SearchUsingQuery(query, userID, 0, 50));
            }

            //IList<long> tagIDs = new List<long>();
            //if (tagFilter != "")
            //{
            //    string[] tags = tagFilter.Split(',');
            //    tagIDs =  TagHelper.TryGetTagIDs(tagFilter.Split(','));
            //}
            //if(tab == 0)
            //    return PartialView(new SourceHelper().GetSourceItems(keywordFilter, tagIDs, userID, true));
            //else
            //    return PartialView("SourceItemsWithUser",new SourceHelper().GetSourceItems(keywordFilter, tagIDs, userID, false));
        }

        public bool DeleteSource(long sourceUserID)
        {
            SourceRepository sourceRepository = new SourceRepository();

            SourceUser sourceuser = sourceRepository.GetSourceUser(sourceUserID);

            if (sourceuser != null && sourceuser.UserID == Utility.GetCurrentUserID())
            {
                return sourceRepository.DeleteSourceUser(sourceuser);

            }
            return false;
        }
    }
}