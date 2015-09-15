using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended
{
    public class SourceData
    {
        public long ID;
        public string Title;
        public string Url;
        public string Summary;
        public IList<Tag> Tags;
        public string UserName;
    }
}
