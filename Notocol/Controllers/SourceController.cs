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
        public ActionResult SourceItems(string query = null, string tagFilter = null, int sourceType = ElasticSearchTest.SOURCE_TYPE_ALL)
        {

            long userID = Utility.GetCurrentUserID();
            
            ElasticSearchTest es = new ElasticSearchTest();
            
            SearchFilter filter = new SearchFilter
            {
                tags = (tagFilter != null && tagFilter.Length > 0) ? tagFilter.Split(new char[] { ',' }) : null
            };




            if (query == null || query.Length == 0)
            {
                return PartialView("ESSource", es.GetSource(filter, userID, 
                    sourceType == ElasticSearchTest.SOURCE_TYPE_OWN? true:false, 0, 50));
                
            }else{
                return PartialView("ESSource", es.SearchUsingQuery(query, filter, userID, sourceType, 0, 50));
            }

        }

        [HttpDelete]
        public bool DeleteSourceUser(long sourceUserID)
        {
            SourceHelper sourceHelper = new SourceHelper();
            return sourceHelper.DeleteSourceUser(sourceUserID, Utility.GetCurrentUserID());

        }
    }
}