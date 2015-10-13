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
        public ActionResult SourceItems(string query = null, string tagFilter = null, int sourceType = ElasticSearchTest.SOURCE_TYPE_ALL)
        {

            long userID = Utility.GetCurrentUserID();
            
            ElasticSearchTest es = new ElasticSearchTest();
            
            SourceListFilter filter = new SourceListFilter
            {
                tags = (tagFilter != null && tagFilter.Length > 0) ? tagFilter.Split(new char[] { ',' }) : null,
                query = query
            };




            if (query == null || query.Length == 0)
            {
                return PartialView("ESSource", es.GetSource(filter, userID, 
                    sourceType == ElasticSearchTest.SOURCE_TYPE_OWN? true:false, 0, 50));
                
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

            ElasticSearchTest es = new ElasticSearchTest();
            //if (filter.query == null || filter.query.Length == 0)
            //{
            //    return es.GetSource(filter, userID,
            //        sourceType == ElasticSearchTest.SOURCE_TYPE_OWN ? true : false, 0, 50);

            //}
            //else
            //{
            //    return es.SearchUsingQuery(filter, userID, sourceType, 0, 50);
            //}

            List<ESSource> testTestList = new List<ESSource>();
            ESSource test = new ESSource();
            test.title = "How long do you think are you gonna last?";
            test.tnImage = "http://www.economictimes.indiatimes.com/photo/29458859.cms";
            test.tnText = "We can see from above that how the directive got created with = and its uses. The details of three points (in pic) are 1- talkinfo here is the object that that got received via talkdetails. You can see, I have accessed the value of talkinfo three times via its properties. 2- talkdetails is attribute name that is used to pass the object via directive. Similar as earlier if we don’t provide the attr as scope : { talkinfo: ‘=’ } then the attribute name will be talkinfo only. 3- talk is the scope object that is assigned to talkdetails";

            test.publicUserNames = new string[]{"apurva", "nilesh", "yashu", "preeti"};
            test.sourceUserID = 34;
            test.tags = new string[]{"check", "test", "debug"};
            test.link = "http://hrm.rtbi.in/index.php?flag=1";
            testTestList.Add(test);
            test.title = "I dont think ber very long";
            testTestList.Add(test);
            return Json(testTestList, JsonRequestBehavior.AllowGet);
         
        }
    }
}