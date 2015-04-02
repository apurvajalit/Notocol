using Model;
using Newtonsoft.Json;
using Notocol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Notocol.Models
{
    public class NewAnnotationDataFromRequest
    {
        public IList<AnnotationRange> ranges;
        public string quote;
        public string text;
        public IList<string> tags;
        public string uri;
        public long user;

        public Annotation toAnnotation()
        {
            Annotation annotation = new Annotation();
            annotation.Ranges = JsonConvert.SerializeObject(this.ranges);
            annotation.Quote = this.quote;
            annotation.Text = this.text;
            annotation.Tags = JsonConvert.SerializeObject(this.tags);
            annotation.Uri = this.uri;
            annotation.User = this.user;

            annotation.Annotator_schema_version = "v1.0";
            annotation.Created = DateTime.UtcNow;
            annotation.Updated = annotation.Created;
            annotation.Consumer = "chimpu";
            annotation.Permissions = JsonConvert.SerializeObject(new AnnotationPermissions());
            annotation.SourceID = 0;
            return annotation;

        }
    }
}
