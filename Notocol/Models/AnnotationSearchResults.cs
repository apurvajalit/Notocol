using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notocol.Models
{
    public class AnnotationSearchResults
    {
        public String total { get; set; }
       
        public IList<Annotation> annotations { get; set; }
    }
}