using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended.Extension
{
    public class ExtensionSearchResponse
    {
        public int total = 0;
        public IList<ExtensionAnnotationData> rows = new List<ExtensionAnnotationData>();

    }
}
