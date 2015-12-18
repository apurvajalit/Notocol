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
using Newtonsoft.Json;
using Model.Extended.View;

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
                case "longpagetile":
                    return PartialView("~/Views/Source/Partials/LongPageTile.cshtml");
                case "pagetilegrid":
                    return PartialView("~/Views/Source/Partials/PageTileGrid.cshtml");
                case "sourcedetails":
                    return PartialView("~/Views/Source/Partials/SourceDetails.cshtml");
                default:
                    throw new Exception("template not known");
            }

        }
        [HttpPost]
        public JsonResult GetSourceList(SourceListFilter filter, int sourceType = ElasticSearchTest.SOURCE_TYPE_ALL)
        {
            long userID = Utility.GetCurrentUserID();
            
            ElasticSearchTest es = new ElasticSearchTest();
            List<PageDetails> results = null;
            SourceHelper sourceHelper = new SourceHelper();
            results = sourceHelper.GetSource(filter, sourceType, userID, 0, 50);
            return Json(results, JsonRequestBehavior.AllowGet);
            
            
        }

        [HttpGet]
        public ContentResult GetSourceUserWithNotes(long sourceUserID, bool userOnly = false)
        {
            var list = JsonConvert.SerializeObject(
                new SourceHelper().GetSourceUserWithNotes(sourceUserID, userOnly, Utility.GetCurrentUserID()),
                Formatting.None,
                new JsonSerializerSettings() {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });

            return Content(list, "application/json");
            
        }

        public ContentResult GetSourceWithNotes(long sourceID)
        {
            var list = JsonConvert.SerializeObject(
                new SourceHelper().GetSourceWithNotes(sourceID, Utility.GetCurrentUserID()),
                Formatting.None,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                });

            return Content(list, "application/json");

        }

        public ActionResult SourceUserNotes(long sourceUserID)
        {
            return View("SourceNotes", new SourceHelper().GetSourceUserWithNotes(sourceUserID, false, Utility.GetCurrentUserID()));
        }

        public ActionResult SourceNotes(long sourceID)
        {
            return View("SourceNotes", new SourceHelper().GetSourceWithNotes(sourceID, Utility.GetCurrentUserID()));
        }

        public JsonResult GetSourceForProfile(long userID)
        {
            SourceHelper sh = new SourceHelper();
            var data = sh.GetProfileSourceData(userID, 0, 20);
            
            return Json(data, JsonRequestBehavior.AllowGet);

        }
    }
}