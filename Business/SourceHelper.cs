using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Repository;
using Model.Extended;
using Model.Extended.Extension;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Security.Cryptography;
using log4net;
using Model.Extended.View;
using Repository.Search;

namespace Business
{
    public class SourceHelper
    {
        static SourceRepository obSourceRepository = new SourceRepository();

        public IList<SourceData> GetSourceItems(string keywordFilter, IList<long> tagIDs , long userID, bool onlyUserSource = true){

            IList<SourceData> retList = new List<SourceData>();
            //if(onlyUserSource)
            //    return obSourceRepository.Search(keywordFilter, tagIDs, userID);
            //else { 
                
            //    return obSourceRepository.Search(keywordFilter, tagIDs);
            //}
            return retList;
        }
        public class ImageInfoComparer : IComparer<PageImageInfo>
        {
            public int Compare(PageImageInfo q, PageImageInfo r)
            {
                return (r.score - q.score);
            }
        }
        private string GetThumbNailImage(string pageURL, PageImageInfo[] pageImageInfo)
        {
            Regex regex = new Regex(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)");
            Match match = regex.Match(pageURL);
            if (match.Success)
            {
                string video_id = match.Groups[1].Value; ;
                string imageURL = "http://img.youtube.com/vi/" + video_id + "/hqdefault.jpg";
                return imageURL;
            }

            pageImageInfo = pageImageInfo.Where(pageImage =>
                                                        !pageImage.hidden &&
                                                        pageImage.width > 100 &&
                                                        pageImage.height >= 100 &&
                                                        (pageImage.url.Contains(".jpg") || pageImage.url.Contains(".png") || pageImage.url.Contains(".gif"))
                                                        ).ToArray<PageImageInfo>();

            if (pageImageInfo.Length <= 0) return null;

            
            int i = 0;
            int max_area = 0, max_area_index = 0;
            foreach (PageImageInfo page in pageImageInfo)
            {
                
                string imageServer = new Uri(page.url).Host;
                imageServer = imageServer.Split(new char[] { '.' }, 2)[1];
                
                if (AdServer.isAdServer(imageServer)) page.score = -1;
                else
                {
                    
                    page.score += (pageImageInfo.Length - i);
                    if (max_area < (page.height * page.width))
                    {
                        max_area = (page.height * page.width);
                        max_area_index = i;
                    }
                }
                
                i++;
            }
            

            if (pageImageInfo[max_area_index].score > 0) pageImageInfo[max_area_index].score += 3;

            pageImageInfo = pageImageInfo.Where(pageImage =>
                                                    pageImage.score > 0).ToArray<PageImageInfo>();
            
            Array.Sort(pageImageInfo, new ImageInfoComparer());
            
            IList<string> imageLinks = new List<string>();

            imageLinks = (from imageLink in pageImageInfo
                          select imageLink.url).ToList();
            
            if (imageLinks.Count > 0) return imageLinks[0];
            
            return null;
        }

        public List<PageDetails> GetSource(SourceListFilter filter, int sourceType, long userID, int offset, int size)
        {
            List<PageDetails> ret = new List<PageDetails>();
            ElasticSearchTest es = new ElasticSearchTest();
            if (filter.query == null || filter.query.Length == 0)
            {
                List<ESSource> results = null;
                if (sourceType == ElasticSearchTest.SOURCE_TYPE_OWN){
                    results = es.GetOwnSource(filter, userID, 0, 50);
                }
                else
                {
                    results = es.GetSourceFromOthers(filter, userID, 0, 50);
                }
                foreach(var res in results)
                {
                    ret.Add(new PageDetails
                    {
                        faviconURL = res.faviconURL,
                        Id = res.Id,
                        sourceUserID = res.sourceUserID,
                        tags = res.tags != null ? res.tags.ToList() : null,
                        thumbnailImageUrl = res.tnImage,
                        thumbnailText = res.tnText,
                        title = res.title,
                        url = res.link,
                        users = res.publicUserNames != null ? res.publicUserNames.ToList() : null
                    });
                }
            }
            else
            {
                ret = es.GetLongSource(filter, userID, sourceType, 0, 50);
           }

            return ret;
        }

        private string GetThumbNailText(string[] pageText, int maxThumbnailTextLength)
        {
            string pageThumbnailText = "";
            int numCharsLeft = maxThumbnailTextLength;
            Array.Sort(pageText, (x, y) => y.Length.CompareTo(x.Length));
            
            foreach (string text in pageText)
            {
                string textToUse = text.Trim();
                if (textToUse.Length > 0)
                {
                    pageThumbnailText = string.Concat(pageThumbnailText, textToUse.Substring(0, numCharsLeft < textToUse.Length ? numCharsLeft : textToUse.Length - 1), System.Environment.NewLine);
                    numCharsLeft = maxThumbnailTextLength - pageThumbnailText.Length;
                    if (numCharsLeft <= 0) break;
                }

            }

            return pageThumbnailText;
        }
        public void SetPageThumbNailData(long userID, ThumbnailDataForSourceUser thumbnailData)
        {
            
            SourceUser sourceUser = obSourceRepository.GetSourceUser(thumbnailData.sourceUserID);
            if (sourceUser == null) return;
            
            int MaxThumbnailTextLength = 500;


            PageImageInfo[] pageImageInfo = JsonConvert.DeserializeObject<PageImageInfo[]>(thumbnailData.imageObjects);
            
            string[] pageText = JsonConvert.DeserializeObject<string[]>(thumbnailData.textData);
            //--
            string thumbnailImageURL = GetThumbNailImage(thumbnailData.pageLink, pageImageInfo);
            
            if (thumbnailImageURL != null) MaxThumbnailTextLength = 200;
            
            string thumbnailImageText = GetThumbNailText(pageText, MaxThumbnailTextLength);
            
            sourceUser.thumbnailImageUrl = thumbnailImageURL;
            sourceUser.thumbnailText = thumbnailImageText;
            
            obSourceRepository.UpdateSourceUser(sourceUser);

            return;
        }
        
        internal SourceUser GetSourceUser(string URI, string sourceLink, long userID)
        {
            return obSourceRepository.GetSourceUser(URI, sourceLink, userID);
        }

        internal Source GetSource(string sourceURI, string sourceLink)
        {
            return obSourceRepository.GetSource(sourceURI, sourceLink);
        }

        internal Source AddSource(string sourceURI, Source source)
        {
            source = obSourceRepository.AddSource(sourceURI, source);
            
            return source;
        }

        internal SourceUser AddSourceUser(SourceUser sourceUser, string username, bool notify = false)
        {
            sourceUser.PrivateNoteCount = sourceUser.noteCount = 0;
            sourceUser = obSourceRepository.AddSourceUser(sourceUser, username);
            if (notify)
            {
                NotificationHelper n = new NotificationHelper();
                n.UpdateNotifications(sourceUser, NotificationHelper.NOTIFICATION_REASON_PAGE_SAVE);
            }

            return sourceUser;
        }

        internal SourceUser UpdateSourceUser(SourceUser sourceUser, string username)
        {
            return obSourceRepository.UpdateSourceUser(sourceUser, username);
        }

        public SourceDataForExtension SaveSource(SourceDataForExtension sourceData, long userID, string username)
        {
            bool notificationRequired = false;
            int notifReason = 0;
            SourceUser sourceUser = null;
            TagHelper tagHelper = new TagHelper();
            FolderHelper folderHelper = new FolderHelper();
            string note = null;

            if (sourceData.sourceUserID > 0)
            {
                sourceUser = obSourceRepository.GetSourceUser(sourceData.sourceUserID);
                
                if (sourceUser == null) {
                    //Call self to create sourceUser for data
                    sourceData.sourceUserID = 0;
                    return SaveSource(sourceData, userID, username);
                }

                sourceUser.FolderID = (sourceData.folderData == null)? 0 : 
                    (sourceData.folderData.selectedFolder == null? 0 : 
                       Convert.ToInt64(sourceData.folderData.selectedFolder.folderID));

                if (sourceUser.Summary != sourceData.summary)
                {
                    if (sourceData.summary != null && sourceData.summary.Length > 0)
                    {
                        notificationRequired = true;
                        notifReason |= NotificationHelper.NOTIFICATION_REASON_NOTE;
                        note = sourceData.summary;
                    }
                    sourceUser.Summary = sourceData.summary;
                }

                if (sourceUser.Privacy != sourceData.privacy)
                {
                    sourceUser.Privacy = sourceData.privacy;
                    sourceUser.PrivacyOverride = true;
                    new NotificationHelper().DeleteNotificationForSourceUser(sourceUser.ID);
                    notificationRequired = false;
                }
                
                tagHelper.UpdateSourceTags(sourceUser, sourceData.tags);
                sourceUser = UpdateSourceUser(sourceUser, username);
            }
            else
            {
                if (sourceData.sourceID <= 0)
                {
                    Source source = obSourceRepository.GetSource(sourceData.uri, sourceData.url);
                    if (source == null)
                    {
                        source = new Source();
                        source.title = sourceData.title;
                        source.faviconURL = sourceData.faviconUrl;
                        source.url = sourceData.url;
                        source = AddSource(sourceData.uri, source);
                    }    

                    if (source == null || source.ID <= 0) return null;
                    sourceData.sourceID = source.ID;
                }

                sourceUser = obSourceRepository.GetSourceUser(sourceData.sourceID, userID);
                if (sourceUser == null)
                {
                    notifReason |= NotificationHelper.NOTIFICATION_REASON_PAGE_SAVE;

                    sourceUser = new SourceUser();
                    sourceUser.SourceID = sourceData.sourceID;

                    sourceUser.FolderID = (sourceData.folderData == null) ? 0:
                    (sourceData.folderData.selectedFolder == null ? 0 :
                        Convert.ToInt64((sourceData.folderData.selectedFolder.folderID))); 

                    sourceUser.Summary = sourceData.summary;
                    if (sourceUser.Summary != null && sourceUser.Summary.Length > 0)
                    {
                        notifReason |= NotificationHelper.NOTIFICATION_REASON_NOTE;
                        note = sourceUser.Summary;
                    }

                    sourceUser.Privacy = sourceData.privacy;
                    if (sourceData.privacy == true)
                    {
                        notificationRequired = false;
                        sourceUser.PrivacyOverride = true;
                    }
                    else
                    {
                        notificationRequired = true;
                    }

                    sourceUser.UserID = userID;
                    sourceUser.noteCount = 0;
                    sourceUser = AddSourceUser(sourceUser, username);
                }
                    
                if (sourceUser == null || sourceUser.ID <= 0) return null;

                if (sourceData.tags != null)
                {
                    tagHelper.UpdateSourceTags(sourceUser, sourceData.tags);
                }
                sourceData.sourceUserID = sourceUser.ID;

            }
            if (notificationRequired)
            {
                new NotificationHelper().UpdateNotifications(sourceUser, notifReason, null, note);
            }
            return sourceData;
        }

        public SourceDataForExtension GetSourceDataForExtension(string URI, string Link, long userID)
        {
            SourceDataForExtension sourceDataForExtension = new SourceDataForExtension();

            sourceDataForExtension.sourceID = obSourceRepository.GetSourceID(URI, Link);
            if (sourceDataForExtension.sourceID <= 0) return sourceDataForExtension;

            SourceUser sourceUser = obSourceRepository.GetSourceUser(sourceDataForExtension.sourceID, userID);
            if (sourceUser != null)
            {
                sourceDataForExtension.sourceUserID = sourceUser.ID;
                sourceDataForExtension.summary = sourceUser.Summary;
                sourceDataForExtension.privacy = (sourceUser.Privacy != null) ? (bool)sourceUser.Privacy : false;
                sourceDataForExtension.noteCount = sourceUser.noteCount;
                if(sourceUser.FolderID != null){
                    sourceDataForExtension.folderData = new FolderDataFromExtension();
                    sourceDataForExtension.folderData.selectedFolder = new FolderDataFromExtensionSelectedFolder();
                    sourceDataForExtension.folderData.selectedFolder.folderID = ((long)sourceUser.FolderID).ToString();
                    sourceDataForExtension.folderData.selectedFolder.folderName =
                        new FolderHelper().GetFolderName(
                        (long)sourceUser.FolderID,
                        userID);
                }
                    
                sourceDataForExtension.tags = new TagHelper().GetSourceTags(sourceUser.ID);
            }

            return sourceDataForExtension;
        }

        public bool DeleteSourceUser(long sourceUserID, long userID, string username)
        {
            SourceRepository sourceRepository = new SourceRepository();

            SourceUser sourceuser = sourceRepository.GetSourceUser(sourceUserID);

            if (sourceuser != null && sourceuser.UserID == userID)
            {
                return sourceRepository.DeleteSourceUser(sourceuser, username);

            }
            return false;
        }

        public SourceUser GetSourceUser(long id)
        {
            return obSourceRepository.GetSourceUser(id);
        }

        public void IncPrivateNoteCount(SourceUser su)
        {
            
            su.PrivateNoteCount++;
            obSourceRepository.UpdateSourceUser(su);
        }

        public void DecNoteCount(SourceUser su, bool isPrivate, string username)
        {
            
            su.noteCount--;
            if(isPrivate)su.PrivateNoteCount--;
            if (su.PrivateNoteCount == 0 && (su.PrivacyOverride == null || !(bool)su.PrivacyOverride)) 
                su.Privacy = false;

            obSourceRepository.UpdateSourceUser(su, username);

        }



        internal void DecPrivateNoteCount(SourceUser su, string username)
        {
            su.PrivateNoteCount--;
            if (su.PrivateNoteCount == 0 && (su.PrivacyOverride == null || !(bool)su.PrivacyOverride)) 
                su.Privacy = false;

            obSourceRepository.UpdateSourceUser(su, username);
        }

        public void SendEventNewPageAdded(SaveSourceData saveSourceData, string userName, bool firstSave)
        {
            
            Dictionary<string, object> trackingValues = new Dictionary<string, object>();

            if (!firstSave)
            {
                trackingValues.Add("tagsUsed", (saveSourceData.sourceData.tags != null && saveSourceData.sourceData.tags.Count > 0));
                trackingValues.Add("folderUsed",
                    (saveSourceData.sourceData.folderData != null && saveSourceData.sourceData.folderData.selectedFolder != null &&
                      saveSourceData.sourceData.folderData.selectedFolder.folderID != "0"
                    ));
                trackingValues.Add("summaryAdded", saveSourceData.sourceData.summary != null);
                trackingValues.Add("privacy", saveSourceData.sourceData.privacy);
            }
            
            
            trackingValues.Add("firstSave", firstSave);
            
            string eventName = "Saved Page";
            
            new ActivityTracker().TrackEvent(eventName, userName, trackingValues);

        }

        public List<NoteData> GetSourceSummaries(long sourceID, long userID)
        {
            List<NoteData> ret = null;
            if (userID != 0){
                ret = obSourceRepository.GetSourceSummarysWithUserAtTop(sourceID, userID);
            }else{
                return new List<NoteData>();
                
            }
            return null;
            //return new List<NoteData>();
        }

        internal List<long> GetSourceUsers(long sourceID)
        {
            return obSourceRepository.GetSourceUsers(sourceID);
        }

        private Dictionary<long, List<NoteData>> GetSourceNotes(long sourceID, long currentUserID) {

            Dictionary<long, List<NoteData>> ret = new Dictionary<long,List<NoteData>>();

            Dictionary<long, NoteData> summaries = obSourceRepository.GetSourceSummaries(sourceID, currentUserID);
            
            List<Annotation> annotations = new AnnotationHelper().GetFullAnnotationWithUserForSource(sourceID, currentUserID);


            foreach (var summary in summaries)
            {
                ret.Add(summary.Key, new List<NoteData>{summary.Value});
            }

            if (annotations.Count > 0)
            {
                int iterator = 0;
                AnnotationHelper annotationHelper = new AnnotationHelper();
                do
                {
                    List<NoteData> currentValue;
                    if (!ret.TryGetValue(annotations[iterator].UserID, out currentValue))
                    {
                        currentValue = new List<NoteData>();
                    }
                    do
                    {
                        currentValue.Add(annotationHelper.GetNoteData(annotations[iterator]));
                        iterator++;
                    } while (iterator < annotations.Count && annotations[iterator].UserID == annotations[iterator].UserID);

                    ret.Add(annotations[iterator].UserID, currentValue);
                } while (iterator < annotations.Count);
            }

            return ret;
        }

        public SourceInfoWithUserNotes GetSourceUserWithNotes(long sourceUserID, bool userOnly, long ownUserID)
        {
            SourceInfoWithUserNotes sourceWithNotes = new SourceInfoWithUserNotes();
            
            sourceWithNotes.sourceUser = obSourceRepository.GetSourceUserWithSource(sourceUserID);
            if (sourceWithNotes.sourceUser == null) return sourceWithNotes;
            sourceWithNotes.source = sourceWithNotes.sourceUser.Source;

            sourceWithNotes.sourceUser.Source = null;
            sourceWithNotes.sourceUser.User = null;
            sourceWithNotes.source.SourceUsers = null;

            if (((bool)sourceWithNotes.sourceUser.Privacy) && (sourceWithNotes.sourceUser.UserID != ownUserID)) return sourceWithNotes;

            sourceWithNotes.userNotes = GetSourceNotes(sourceWithNotes.source.ID, ownUserID);
            TagHelper tagHelper = new TagHelper();
            
            sourceWithNotes.tags = tagHelper.GetSourceUserTags(sourceUserID);
            
            
            return sourceWithNotes;

        }

        public SourceInfoWithUserNotes GetSourceWithNotes(long sourceID, long ownUserID)
        {
            SourceInfoWithUserNotes sourceWithNotes = new SourceInfoWithUserNotes();

            sourceWithNotes.source = obSourceRepository.GetSource(sourceID);
            if (sourceWithNotes.source == null) return sourceWithNotes;
            TagHelper tagHelper = new TagHelper();
            sourceWithNotes.tags = tagHelper.GetSourceTags(sourceID);
            sourceWithNotes.userNotes = GetSourceNotes(sourceWithNotes.source.ID, ownUserID);

            return sourceWithNotes;
        }

        public List<PageDetails> GetProfileSourceData(long userID, int offset, int size)
        {
            var data = obSourceRepository.GetProfileSourceData(userID, offset, size);
            List<PageDetails> sourceData = new List<PageDetails>();
            AnnotationHelper annHelper = new AnnotationHelper();
            
            if (data != null)
            {
                PageDetails psource = null;
                int i = 0;
                while(i < data.Count)
                {
                    if (psource != null) sourceData.Add(psource);
                    var row = data[i];
                    long currentSourceUserID = row.sourceUserID;

                    psource = new PageDetails
                    {
                        title = row.title,
                        faviconURL = row.faviconURL,
                        thumbnailImageUrl = row.thumbnailImageUrl,
                        thumbnailText = row.thumbnailText,
                        Id = (long)row.sourceID,
                        sourceUserID = row.sourceUserID,
                        url = row.url,
                        tags = new List<string>(),
                        notes = new List<NoteDisplay>()
                    };

                    if(row.Summary != null && row.Summary.Length > 0)
                    {
                        psource.notes.Add(new NoteDisplay
                        {
                            text = row.Summary
                        });
                    }
                    do
                    {
                        row = data[i];
                        if (row.tag != null) psource.tags.Add(row.tag);
                        
                        if ((row.Text != null || row.Target != null) && 
                            !annHelper.IsAnnotationPrivate(row.Permissions))
                        {
                            psource.notes.Add(new NoteDisplay
                            {
                                text = row.Text,
                                quote = (row.Target == null)?null:annHelper.GetNoteQuote(row.Target)
                            });
                        }
                        i++;
                    } while (( i < data.Count) && currentSourceUserID == data[i].sourceUserID);
                }

                if (psource != null) sourceData.Add(psource);
            }
            return sourceData;
        }
    }

}