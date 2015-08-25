
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model.Extended.Extension;
using Model;
using Repository;
using Newtonsoft.Json;

namespace Business
{
    public class AnnotationHelper
    {
        AnnotationRepository objAnnotationRepository = new AnnotationRepository();
        public ExtensionAnnotationData AnnotationToExtensionAnnotation(Annotation annotation)
        {
            ExtensionAnnotationData extAnnData = new ExtensionAnnotationData();

            extAnnData.updated = annotation.Updated;

            if (annotation.Target != null)
                extAnnData.target = JsonConvert.DeserializeObject<Target[]>(annotation.Target);
            extAnnData.created = annotation.Created;
            extAnnData.text = annotation.Text;

            if (annotation.Tags != null)
                extAnnData.tags = JsonConvert.DeserializeObject<string[]>(annotation.Tags);
            extAnnData.uri = annotation.Uri;
            extAnnData.document = JsonConvert.DeserializeObject(annotation.Document);
            extAnnData.consumer = annotation.Consumer;
            extAnnData.id = annotation.ID;
            extAnnData.user = annotation.User;
            extAnnData.permissions = JsonConvert.DeserializeObject(annotation.Permissions);

            return extAnnData;
        }

        public Annotation ExtensionAnnotationToAnnotation(ExtensionAnnotationData extAnnotation)
        {

            Annotation annotation = new Annotation();
            annotation.Updated = extAnnotation.updated;
            if (extAnnotation.target != null && extAnnotation.target.Count() > 0)
                annotation.Target = JsonConvert.SerializeObject(extAnnotation.target);

            annotation.Created = extAnnotation.created;
            annotation.Text = extAnnotation.text;
            annotation.Uri = extAnnotation.uri;
            if (extAnnotation.tags != null && extAnnotation.tags.Count() > 0)
                annotation.Tags = JsonConvert.SerializeObject(extAnnotation.tags);

            annotation.Document = JsonConvert.SerializeObject(extAnnotation.document);
            annotation.Consumer = extAnnotation.consumer;
            annotation.ID = extAnnotation.id;
            annotation.Permissions = JsonConvert.SerializeObject(extAnnotation.permissions);
            annotation.User = extAnnotation.user;

            return annotation;
        }
        public ExtensionSearchResponse GetAnnotationsForPage(long userID, string uri)
        {
            ExtensionSearchResponse res = new ExtensionSearchResponse();
            

            List<Annotation> annotationList = objAnnotationRepository.GetAnnotationsForPage(uri, userID);

            res.total = annotationList.Count;
            foreach (var annotation in annotationList)
                res.rows.Add(AnnotationToExtensionAnnotation(annotation));

            return res;
        }

        public ExtensionAnnotationData AddAnnotation(ExtensionAnnotationData extAnnotation, string userName, long userID)
        {
            extAnnotation.created = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            extAnnotation.updated = extAnnotation.created;
            extAnnotation.consumer = userName;
            Annotation annotation = ExtensionAnnotationToAnnotation(extAnnotation);

            //TODO Take care of the following
            annotation.UserID = (int)userID;

            if ((annotation.ID = objAnnotationRepository.AddAnnotation(annotation)) <= 0)
            {
                return null; //TODO add a more informative error
            }
            long sourceID = annotation.SourceID != null? (long) annotation.SourceID:0;
            if (sourceID > 0)
            {
                SourceHelper sourceHelper = new SourceHelper();
                Source source = sourceHelper.GetSource(sourceID);
                source.noteCount++;
                sourceHelper.UpdateSource(source);
            }

            extAnnotation.id = annotation.ID;
            return extAnnotation;
        }

        public ExtensionAnnotationData GetAnnotation(long annotationID)
        {
            Annotation annotation = null;
            if ((annotation = objAnnotationRepository.GetAnnotation(annotationID)) != null)
                return AnnotationToExtensionAnnotation(annotation);

            return null;
        }

        public ExtensionAnnotationData UpdateAnnotation(long id, ExtensionAnnotationData extAnnotation, long userID)
        {
            Annotation updatedannotation = ExtensionAnnotationToAnnotation(extAnnotation);
            Annotation annotation = objAnnotationRepository.GetAnnotation(id);
            if (annotation == null || annotation.UserID != userID)
                return null;

            annotation.Updated = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            annotation.Text = updatedannotation.Text;
            annotation.Tags = updatedannotation.Tags;
            annotation.Permissions = updatedannotation.Permissions;
            annotation.Document = updatedannotation.Document;

            //annotation.UserID = (int)userID;
            annotation.ID = (int)id;


            if (objAnnotationRepository.UpdateAnnotation(annotation))
                return extAnnotation;

            return null;
        }

        public bool DeleteAnnotation(long annotationID, long userID)
        {
            Annotation annotation = objAnnotationRepository.GetAnnotation(annotationID);
            if (annotation == null || annotation.UserID != userID) return false;

            return objAnnotationRepository.DeleteAnnotation(annotationID);

        }
    }
}
