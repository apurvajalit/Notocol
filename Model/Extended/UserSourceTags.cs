using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class UserSourceTags
    {
        public int UserID { get; set; }
        public Source Source { get; set; }
        public IList<Tag> Tags { get; set; } 

    }
}
