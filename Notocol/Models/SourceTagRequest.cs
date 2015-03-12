using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model;

namespace Notocol.Models
{
    
    public class SourceTagRequest
    {
        public Source Source { get; set; }
        public IList<Tag> Tags { get; set; }
    }
}