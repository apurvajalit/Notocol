using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended.Extension
{
    public class PageImageInfo
    {
        public string url { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public bool hidden { get; set; }

        public int score { get; set; }
    }
}
