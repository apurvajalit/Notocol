using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended.Extension
{
    public class SourceDataForExtension
    {
        public string urn { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string summary { get; set; }
        public string faviconUrl { get; set; }
        public string[] tags { get; set; }
        public long folder { get; set; }
        public long sourceID { get; set; }

        public int noteCount { get; set; }
    }
}
