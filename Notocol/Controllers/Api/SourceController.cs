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

namespace Notocol.Controllers.Api
{
    public class SourceController : BaseApiController
    {
        [HttpPost]
        
        public long UpdateSource([FromBody]SourceDataForExtension sourceData)
        {
            return new SourceHelper().AddOrUpdateSourceFromExtension(Utility.GetCurrentUserID(), sourceData);
            
        }

        
        [HttpGet]

        public SourceDataForExtension SourceData(string pageURL)
        {
            long userID = Utility.GetCurrentUserID();
            if(userID <= 0 ) return null;
            return new SourceHelper().GetSourceExtensionData(pageURL, userID);

            
        }
        

    }
}
