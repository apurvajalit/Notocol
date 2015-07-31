using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended.Extension
{
    public class SourceDataForBookmark
    {
        public string sourceLink { get; set; }
        public string sourceURI { get; set; }
        public string title { get; set; }
        public string summary { get; set; }
        public string[] tags { get; set; }
    }
}
