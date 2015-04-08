using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

//using System.IdentityModel.Tokens;
using Notocol.Models;
using Model;

namespace Notocol.Controllers.Api
{
        
    public class AnnotationController : BaseApiController
    {
        [HttpGet]
        public string Annotation()
        {
            AnnotaterStoreInfo storeInfo = new AnnotaterStoreInfo();
            //return Json(storeInfo, JsonRequestBehavior.AllowGet);
            return JsonConvert.SerializeObject(storeInfo);
        }

        [HttpPost]
        public string Annotation(Annotation obj)
        {
            return JsonConvert.SerializeObject(obj); ;
        }


        
    }
}