using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notocol.Models
{
    /*
     * "start": "/p[69]/span/span",           # (relative) XPath to start element
      "end": "/p[70]/span/span",             # (relative) XPath to end element
      "startOffset": 0,                      # character offset within start element
      "endOffset": 120 */
    public class AnnotationRange
    {
        public String start { get; set; }
        public String end { get; set; }
        public String startoffset { get; set; }
        public String endoffset { get; set; }
    }
}