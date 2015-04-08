using Notocol.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Notocol.Controllers.Api
{
    public class SourceDataForExtensionController:BaseApiController
    {
        [HttpGet]
        public SourceDataForExtension GetSourceData(string pageURL, long userID)
        {
            SourceRepository obSourceRepository = new SourceRepository();
            return obSourceRepository.getSourceData(pageURL, userID);
        }

        
    }
}