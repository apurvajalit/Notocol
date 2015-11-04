using Business;
using Model.Extended.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
//using System.Web.Mvc;
using Notocol.Models;
using Newtonsoft.Json;
using log4net;

namespace Notocol.Controllers.Api
{
    public class ThumbnailDataController : BaseApiController
    {
        // post: ThumbnailData
        [HttpPost]
        public void ThumbnailData(ThumbnailDataForSourceUser thumbnailData)
        {
            SourceHelper sourceHelper = new SourceHelper();
            
            sourceHelper.SetPageThumbNailData(Utility.GetCurrentUserID(), thumbnailData);
            return;
        }

    }
}