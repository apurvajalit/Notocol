using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Repository.Search;
using Notocol.Models;

namespace Notocol.Controllers
{
    public class ElasticSearchTestController : Controller
    {
        // GET: ElasticSearchTest
        public ActionResult Index(string searchQuery)
        {
            if (searchQuery != null)
            {
                ElasticSearchTest elasticSearchTest = new ElasticSearchTest();
                //return View(elasticSearchTest.SearchUsingQuery(searchQuery, Utility.GetCurrentUserID(), 0, 20));
                return View();
            }


            return View();
        }
    }
}