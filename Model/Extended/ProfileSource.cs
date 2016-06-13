using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended
{
    public class NoteDataShort
    {
        public string quote { get; set; }
        public string text { get; set; }
        
    }
    public class ProfileSource
    {
        public long sourceID { get; set; }
        public long sourceUserID { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string faviconURL { get; set; }
        public string thumbnailImageUrl { get; set; }
        public string thumbnailText { get; set; }
        public List<string> tags { get; set; }
        public List<NoteDataShort> notes { get; set; }
    }

    
}
