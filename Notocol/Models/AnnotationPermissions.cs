using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notocol.Models
{
    public class AnnotationPermissions
    {
        
            public IList<String> read { get; set; }
            public IList<String> admin { get; set; }
            public IList<String> update { get; set; }
            public IList<String> delete { get; set; }

        
    }
}