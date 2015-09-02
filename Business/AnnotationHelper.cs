
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model.Extended.Extension;
using Model;
using Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public Source GetSourceFromAnnotation(Annotation annotation){
            string sourceURI = null, sourceURN = null;
            var document = JObject.Parse(annotation.Document);
            var links = document["link"];
            Source source = null;
            SourceHelper sourceHelper = new SourceHelper();

            foreach (var link in links)
            {
                if (link["rel"] != null) continue;
                string href = (string)link["href"];
                if (href.Contains("urn:x-pdf:"))
                {
                    sourceURN = href.Substring("urn:x-pdf:".Length);

                }
                else
                {
                    sourceURI = System.Uri.UnescapeDataString(href);
                }

            }

            if (sourceURN != null)
                source = sourceHelper.GetSourceFromURN(sourceURN, annotation.UserID);
            else
                source = sourceHelper.GetSource(sourceURI, annotation.UserID);

            
            
            if (source == null)
            {
                Source sourceFromAnnotation = new Source();
                
                sourceFromAnnotation.Title = (string)document["title"];
                sourceFromAnnotation.FaviconURL = (document["favicon"] != null) ? (string)document["favicon"] : null;
                sourceFromAnnotation.UserID = annotation.UserID ;
                sourceFromAnnotation.SourceURI = sourceURI;
                sourceFromAnnotation.URN = sourceURN;

                //Way to get thumbnail Data

                if (document["facebook"] != null)
                {
                    var documentFacebookData = document["facebook"];
                    if (documentFacebookData["image"] != null)
                    {

                
                    }

                    if (documentFacebookData["description"] != null)
                    {
                
                    }
                }
                source = sourceHelper.Add(sourceFromAnnotation);
               
            }
            return source;
        }
        public ExtensionAnnotationData AddAnnotation(ExtensionAnnotationData extAnnotation, string userName, long userID)
        {

            extAnnotation.created = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            extAnnotation.updated = extAnnotation.created;
            extAnnotation.consumer = userName;
            
            Annotation annotation = ExtensionAnnotationToAnnotation(extAnnotation);
            annotation.UserID = (int)userID;
            Source source = GetSourceFromAnnotation(annotation);
            
            if ((source == null) || ((annotation.ID = objAnnotationRepository.AddAnnotation(annotation)) <= 0))
            {
                return null; //TODO add a more informative error
            }

            source.noteCount++;
            SourceHelper sourceHelper = new SourceHelper();           
            
            sourceHelper.UpdateSource(source);
            

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
