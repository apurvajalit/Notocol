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
using Model.Extended.Extension;

namespace Notocol.Controllers
{
    public class SourceController : Controller
    {
        
        // GET: Source
        public ActionResult SourceItems(string query = null, string user=null, string tagFilter = null, int sourceType = ElasticSearchTest.SOURCE_TYPE_ALL)
        {

            long userID = Utility.GetCurrentUserID();
            
            ElasticSearchTest es = new ElasticSearchTest();
            
            SourceListFilter filter = new SourceListFilter
            {
                tags = (tagFilter != null && tagFilter.Length > 0) ? tagFilter.Split(new char[] { ',' }) : null,
                query = query,
                user = (user == null)?null:user
            };




            if (query == null || query.Length == 0)
            {
                if (sourceType == ElasticSearchTest.SOURCE_TYPE_OWN)
                {
                    return PartialView("ESSource", es.GetOwnSource(filter, userID, 0, 50));
                
                }
                else
                {
                    return PartialView("ESSource", es.GetSourceFromOthers(filter, userID, 0, 50));
                }
                
            }else{
                return PartialView("ESSource", es.SearchUsingQuery(filter, userID, sourceType, 0, 50));
            }

        }

        [HttpDelete]
        public bool DeleteSourceUser(long sourceUserID)
        {
            SourceHelper sourceHelper = new SourceHelper();
            return sourceHelper.DeleteSourceUser(sourceUserID, Utility.GetCurrentUserID(), Utility.GetCurrentUserName());

        }

        public ActionResult Template(string template)
        {
            switch (template.ToLower())
            {
                case "pagetile":
                    return PartialView("~/Views/Source/Partials/PageTile.cshtml");
                case "pagetilegrid":
                    return PartialView("~/Views/Source/Partials/PageTileGrid.cshtml");
                default:
                    throw new Exception("template not known");
            }

        }
        [HttpPost]
        public JsonResult GetSourceList(SourceListFilter filter, int sourceType = ElasticSearchTest.SOURCE_TYPE_ALL)
        {
            long userID = Utility.GetCurrentUserID();
            List<ESSource> results = null;
            ElasticSearchTest es = new ElasticSearchTest();
            
            if (sourceType == ElasticSearchTest.SOURCE_TYPE_OWN)
            {
                results = es.GetOwnSource(filter, userID, 0, 50);
            }else{
                results = es.GetSourceFromOthers(filter, userID, 0, 50);
            }
            
            return Json(results, JsonRequestBehavior.AllowGet);
         
        }
    }
}