using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notocol.Models
{
    public class SourceDataRequest
    {
        public  Source Source { get; set; }
        public IList<Tag> Tags { get; set; }

    }

}
