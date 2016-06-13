using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

//using System.IdentityModel.Tokens;
using Notocol.Models;

namespace Notocol.Controllers.Api
{
        
    public class DepricatedAnnotationController : BaseApiController
    {
        [HttpGet]
        public string Annotation()
        {
            AnnotaterStoreInfo storeInfo = new AnnotaterStoreInfo();
            //return Json(storeInfo, JsonRequestBehavior.AllowGet);
            return JsonConvert.SerializeObject(storeInfo);
        }

        [HttpPost]
        public string Annotation(AnnotationData obj)
        {
            return JsonConvert.SerializeObject(obj); ;
        }


        
    }
}