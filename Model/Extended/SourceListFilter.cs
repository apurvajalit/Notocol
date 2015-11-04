using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended
{
    public class SourceListFilter
    {
        public string query { get; set; }
        public string[] tags { get; set; }
        public long[] folderIDs { get; set; }

        public string user { get; set; }
    }
}
