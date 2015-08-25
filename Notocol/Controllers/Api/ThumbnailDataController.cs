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

namespace Notocol.Controllers.Api
{
    public class ThumbnailDataController : BaseApiController
    {
        // post: ThumbnailData
        [HttpPost]
        public void ThumbnailData(ThumbnailDataFromSource thumbnailData)
        {
            SourceHelper sourceHelper = new SourceHelper();
            //ThumbnailDataFromSource thumbnailData = JsonConvert.DeserializeObject<ThumbnailDataFromSource>(thumbnailDataString);
            sourceHelper.SetPageThumbNailData(Utility.GetCurrentUserID(), thumbnailData);
            return;
        }

        [HttpGet]
        public string ThumbnailData()
        {
            
            return ("I am running");
        }

    }
}