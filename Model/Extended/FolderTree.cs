using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended
{
    public class FolderTree
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public FolderTree Parent { get; set; }
        public string ParentID { get; set; }
        public IList<FolderTree> Children { get; set; }
    }
}
