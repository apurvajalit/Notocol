
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
            {
                ExtensionAnnotationData extAnnotation = AnnotationToExtensionAnnotation(annotation);
                extAnnotation.tags = new TagHelper().GetAnnotationTagNames(annotation.ID);
                res.rows.Add(extAnnotation);
            }
            return res;
        }

        public SourceUser GetSourceUserFromAnnotation(Annotation annotation){
            string sourceLink = null, sourceURI = null;
            var document = JObject.Parse(annotation.Document);
            var links = document["link"];
            SourceUser sourceUser = null;
            SourceHelper sourceHelper = new SourceHelper();

            foreach (var link in links)
            {
                if (link["rel"] != null) continue;
                string href = (string)link["href"];
                if (href.Contains("urn:x-pdf:"))
                {
                    sourceURI = href.Substring("urn:x-pdf:".Length);

                }
                else
                {
                    sourceLink = System.Uri.UnescapeDataString(href);
                }

            }

            if (sourceLink == null) return null;
            if (sourceURI == null) sourceURI = sourceLink;
            
            sourceUser = sourceHelper.GetSourceUser(sourceURI, sourceLink, annotation.UserID);

            if (sourceUser != null) return sourceUser;

            //Create sourceUser since it does not exist
            Source source = sourceHelper.GetSource(sourceURI, sourceLink);
            if (source == null || source.ID <= 0)
            {
                source = new Source();
                source.title = (string)document["title"];
                source.faviconURL = (document["favicon"] != null) ? (string)document["favicon"] : null;
                source.url = sourceLink;

                source = sourceHelper.AddSource(sourceURI, source);

                if (source == null || source.ID <= 0) return null;
            }
            

            sourceUser = new SourceUser();
            sourceUser.SourceID = source.ID;
            sourceUser.UserID = annotation.UserID;
            sourceUser.Privacy = false;

            if (document["facebook"] != null)
            {
                //Way to get thumbnail Data
                var documentFacebookData = document["facebook"];
                if (documentFacebookData["image"] != null && documentFacebookData["image"].HasValues)
                    sourceUser.thumbnailImageUrl = documentFacebookData["image"].First().Value<String>();

                if (documentFacebookData["description"] != null && documentFacebookData["description"].HasValues)
                    sourceUser.thumbnailText = documentFacebookData["description"].First().Value<String>();
                
            }

            sourceUser = sourceHelper.AddSourceUser(sourceUser);
            return sourceUser;
        }

        public ExtensionAnnotationData AddAnnotation(ExtensionAnnotationData extAnnotation, string userName, long userID)
        {

            extAnnotation.created = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            extAnnotation.updated = extAnnotation.created;
            extAnnotation.consumer = userName;
            
            Annotation annotation = ExtensionAnnotationToAnnotation(extAnnotation);
            annotation.UserID = (int)userID;
            SourceUser sourceUser = GetSourceUserFromAnnotation(annotation);
            if (sourceUser == null || sourceUser.ID <= 0 ) return null;

            annotation.SourceUserID = sourceUser.ID;

            if ((annotation.ID = objAnnotationRepository.AddAnnotation(annotation, sourceUser, extAnnotation.tags)) <= 0) return null;

            sourceUser.noteCount++;
            SourceHelper sourceHelper = new SourceHelper();
            sourceHelper.UpdateSourceUser(sourceUser);
            extAnnotation.id = annotation.ID;

            if (extAnnotation.tags != null && extAnnotation.tags.Length > 0)
                new TagHelper().UpdateAnnotationTags(annotation, extAnnotation.tags, (long)sourceUser.SourceID);
            
            return extAnnotation;
        }

        public ExtensionAnnotationData GetAnnotation(long annotationID)
        {
            Annotation annotation = null;
            ExtensionAnnotationData extAnnotation = null;
            if ((annotation = objAnnotationRepository.GetAnnotation(annotationID)) != null)
            {
                extAnnotation = AnnotationToExtensionAnnotation(annotation);
                extAnnotation.tags = new TagHelper().GetAnnotationTagNames(annotation.ID);
            }
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
            annotation.Permissions = updatedannotation.Permissions;
            annotation.Document = updatedannotation.Document;

            //annotation.UserID = (int)userID;
            annotation.ID = (int)id;


            if (!objAnnotationRepository.UpdateAnnotation(annotation)) return null;
            
            new TagHelper().UpdateAnnotationTags(annotation, extAnnotation.tags);
            
            return extAnnotation;

            
        }

        public bool DeleteAnnotation(long annotationID, long userID)
        {
            Annotation annotation = objAnnotationRepository.GetAnnotation(annotationID);
            if (annotation == null || annotation.UserID != userID) return false;

            return objAnnotationRepository.DeleteAnnotation(annotationID);

        }
    }
}
