using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace Notocol.Controllers.Api
{
    public class BaseApiController : ApiController
    {
        private static ILog log;
         public BaseApiController()
        {
            log = LogManager.GetLogger(GetType().Name);
            //logger.Debug("Logger called");
        }
         public void Debug(string msg)
         {
             log.Debug(msg);
         }
    }
}
