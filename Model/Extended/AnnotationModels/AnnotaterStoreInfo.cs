using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notocol.Models
{
    public class AnnotaterStoreInfo
    {
        public string name { get; set; }
        public string version { get; set; }

        public AnnotaterStoreInfo()
        {
            this.name = "Annotator Store API";
            this.version = "2.0.0";
        }
    }
}