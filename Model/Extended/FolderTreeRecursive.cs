using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended
{
    public class FolderTreeRecursive
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public FolderTreeRecursive Parent { get; set; }
        public string ParentID { get; set; }
        public IList<FolderTreeRecursive> Children { get; set; }
    }
}
