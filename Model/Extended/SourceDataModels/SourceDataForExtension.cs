using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model;
using System.Runtime.Serialization;

namespace Notocol.Models
{
    
    

    


    public class SourceDataForExtension
    {
    
        public SourceDetails Source { get; set; }
        
    
        public IList<TagDetails> Tags { get; set; }

       
        public IList<AnnotationDetails> Annotations { get; set; }
        
    }
}