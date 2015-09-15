using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Model;
using Repository;
using Model.Extended.Extension;
using Model.Extended;
using Business;
using Notocol.Models;
using Model.Extended.Extension;
using Newtonsoft.Json.Linq;

namespace Notocol.Controllers.Api
{
    public class SourceController : BaseApiController
    {
        SourceHelper sourceHelper = new SourceHelper();
        
        [HttpPost]
        public JObject SaveSource([FromBody]SourceDataForExtension sourceData)
        {
            
            sourceData = sourceHelper.SaveSource(sourceData, Utility.GetCurrentUserID());
            if (sourceData != null && sourceData.sourceUserID != 0) {
                return JObject.FromObject(new{
                    status = "success",
                    sourceData = sourceData
                });
            }else{
                return JObject.FromObject(new
                {
                    status = "failed",
                });
            }
        }

        
        [HttpGet]

        public JObject GetSourceData(string URI, string Link)
        {
            SourceDataForExtension sourceData = sourceHelper.GetSourceDataForExtension(URI, Link, Utility.GetCurrentUserID());
            return JObject.FromObject(new
            {
                sourceData = sourceData
            });
        }
    }
}
