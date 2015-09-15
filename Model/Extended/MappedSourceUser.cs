using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended
{
    public class MappedSourceUser
    {
        public long SUID { get; set; }
        public long userID { get; set; }
        public long sourceID { get; set; }

        public string Title { get; set; }
        public string link { get; set; }
        public string faviconURL { get; set; }
        public int noteCount { get; set; }
        
        public string thumbnaiImageURL { get; set; }
        public string thumbnailText { get; set; }
        public string summary { get; set; }
        public bool privacy { get; set; }

        public int folder { get; set; }
        public Tag[] tags { get; set; }
        public DateTime lastUsed { get; set; }
    }
}
