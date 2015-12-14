using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended
{
    public class UserProfileInfo
    {
        public string name { get; set; }
        public string userName { get; set; }
        public long ID { get; set; }
        public int followers { get; set; }
        public int follows { get; set; }
        public int numberOfNotes { get; set; }
        public int numberOfPages { get; set; }
        public int  numberofCollections { get; set; }
        public List<string> recentTags { get; set; }
        public List<Folder> collections{ get; set; }
    }
}
