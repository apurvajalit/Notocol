using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended
{
    public class FolderTree
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public long ParentID { get; set; }
        public IList<FolderTree> Children { get; set; }
    }
}
