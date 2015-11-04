using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended
{
    public class FolderTree
    {
        public string  ID { get; set; }
        public string name { get; set; }
        public IList<FolderTree> children { get; set; }
    }
}
