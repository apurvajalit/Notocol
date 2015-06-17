using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace Notocol.Controllers
{
    public abstract class BaseController : Controller
    {
        private static ILog log;
        //readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public BaseController()
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