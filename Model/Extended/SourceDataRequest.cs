using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model;
using System.Runtime.Serialization;

namespace Notocol.Models
{
    [DataContract(IsReference = true)]
    public class SourceDataRequest
    {
        [DataMember]
        public Source Source { get; set; }
        
        [DataMember]
        public IList<Tag> Tags { get; set; }

         [DataMember]
        public IList<AnnotationDataResponse> Annotations { get; set; }
        
    }
}