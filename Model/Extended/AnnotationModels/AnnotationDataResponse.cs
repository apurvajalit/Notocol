using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notocol.Models
{
    public class AnnotationDataResponse
    {
        public IList<AnnotationRange> ranges;
        public string quote;
        public string text;
        public IList<string> tags;
        public string uri;
        public long user;
        public long id;
        public string created;
        public string updated;
        

        public AnnotationDataResponse(Annotation annotation){
            this.id = annotation.ID;
            this.created = "2011-05-24T18:52:08.036814"; //TODO Add the dates here in appropriate format
            this.updated = "2011-05-24T18:52:08.036814";
            this.text = annotation.Text;
            this.quote = annotation.Quote;
            this.uri = annotation.Uri;
            this.ranges = AnnotationRange.AnnotationRangeListFromString(annotation.Ranges);
            this.user = (long) annotation.User;
            
            this.tags = JsonConvert.DeserializeObject<IList<string>>(annotation.Tags);
            
        }
        public AnnotationDataResponse()
        {
            
        }
        public static Annotation GetAnnotationDBObject(AnnotationDataResponse annData)
        {
            Annotation annotation = new Annotation();

            return annotation;
        }

        public static Annotation UpdateAnnotationObject(Annotation annotation, AnnotationDataResponse annData)
        {
            annotation.Text = annData.text;
            annotation.Quote = annData.quote;
            annotation.Tags = JsonConvert.SerializeObject(annData.tags);
            annotation.Ranges = JsonConvert.SerializeObject(annData.ranges);
            annotation.Updated = DateTime.UtcNow;

            return annotation;
        }
    }

}
