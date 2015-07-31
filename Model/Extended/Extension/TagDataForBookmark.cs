using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended.Extension
{
    public class TagEntity
    {
        public string text { get; set; }
    }
    public class TagDataForBookmark
    {
        public string source { get; set; }
        public TagEntity[] tag { get; set; }
        public bool isPrivate { get; set; }
        

    }
}
