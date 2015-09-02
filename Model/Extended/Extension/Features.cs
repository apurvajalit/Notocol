using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended.Extension
{
    public class Features
    {
        public bool claim { get; set; }
        public bool notification { get; set; }
        public bool queue { get; set; }

        public bool streamer { get; set; }

        public bool groups { get; set; }

        public bool search_normalized { get; set; }
    }
}
