using Notocol.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Notocol.Controllers
{
    public class AnnotationController : Controller
    {
        // GET: Annotation
        public IList<AnnotationDataResponse> Annotation(string url, long userID)
        {
            AnnotationRepository objAnnotationRepository = new AnnotationRepository();
            return objAnnotationRepository.getAnnotations(url, userID);
           
        }
    }
}