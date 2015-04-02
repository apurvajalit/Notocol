using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notocol.Models
{
    public class AnnotationSearchResults
    {
        public long total { get; set; }
       
        public IList<AnnotationDataResponse> rows { get; set; }
    }
}