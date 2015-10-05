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

            LogManager.GetLogger(GetType().Name).Debug("1 " + pageImageInfo.Length);
            
            foreach(var temp in pageImageInfo){
                LogManager.GetLogger(GetType().Name).Debug("Values: " + JsonConvert.ToString(temp));
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
            
            LogManager.GetLogger(GetType().Name).Debug("Setting values " + sourceUser.thumbnailImageUrl + " "+ thumbnailImageText);
            
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

        internal SourceUser AddSourceUser(SourceUser sourceUser)
        {
            sourceUser.PrivateNoteCount = sourceUser.noteCount = 0;
            sourceUser = obSourceRepository.AddSourceUser(sourceUser);
            

            return sourceUser;
        }

        internal SourceUser UpdateSourceUser(SourceUser sourceUser)
        {
            return obSourceRepository.UpdateSourceUser(sourceUser);
        }

        public SourceDataForExtension SaveSource(SourceDataForExtension sourceData, long userID)
        {
            SourceUser sourceUser = null;
            TagHelper tagHelper = new TagHelper();
            FolderHelper folderHelper = new FolderHelper();
                 
            
            //Handle folder info here
            //if (sourceData.folderData != null && sourceData.folderData.addedFolders != null)
            //{
            //    sourceData.folderData = folderHelper.ProcessExtensionFolderData(sourceData.folderData, userID);
                
            //}
            

            if (sourceData.sourceUserID > 0)
            {
                sourceUser = obSourceRepository.GetSourceUser(sourceData.sourceUserID);
                
                if (sourceUser == null) {
                    //Call self to create sourceUser for data
                    sourceData.sourceUserID = 0;
                    return SaveSource(sourceData, userID);
                }

                sourceUser.FolderID = (sourceData.folderData == null)? 0 : 
                    (sourceData.folderData.selectedFolder == null? 0 : 
                       Convert.ToInt64(sourceData.folderData.selectedFolder.folderID));

                sourceUser.Summary = sourceData.summary;
                if (sourceUser.Privacy != sourceData.privacy)
                {
                    sourceUser.Privacy = sourceData.privacy;
                    sourceUser.PrivacyOverride = true;
                }
                
                tagHelper.UpdateSourceTags(sourceUser, sourceData.tags);
                sourceUser = UpdateSourceUser(sourceUser);
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
                    sourceUser = new SourceUser();
                    sourceUser.SourceID = sourceData.sourceID;

                    sourceUser.FolderID = (sourceData.folderData == null) ? 0:
                    (sourceData.folderData.selectedFolder == null ? 0 :
                        Convert.ToInt64((sourceData.folderData.selectedFolder.folderID))); 

                    sourceUser.Summary = sourceData.summary;
                    sourceUser.Privacy = sourceData.privacy;
                    if (sourceData.privacy == true) sourceUser.PrivacyOverride = true;

                    sourceUser.UserID = userID;
                    sourceUser.noteCount = 0;
                    sourceUser = AddSourceUser(sourceUser);
                }
                    
                if (sourceUser == null || sourceUser.ID <= 0) return null;

                if (sourceData.tags != null)
                {
                    tagHelper.UpdateSourceTags(sourceUser, sourceData.tags);
                }
                sourceData.sourceUserID = sourceUser.ID;

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

        public bool DeleteSourceUser(long sourceUserID, long userID)
        {
            SourceRepository sourceRepository = new SourceRepository();

            SourceUser sourceuser = sourceRepository.GetSourceUser(sourceUserID);

            if (sourceuser != null && sourceuser.UserID == userID)
            {
                return sourceRepository.DeleteSourceUser(sourceuser);

            }
            return false;
        }

        public SourceUser GetSourceUser(long id)
        {
            return obSourceRepository.GetSourceUser(id);
        }

        public void IncPrivateNoteCount(long id)
        {
            SourceUser su = obSourceRepository.GetSourceUser(id);
            su.PrivateNoteCount++;
            obSourceRepository.UpdateSourceUser(su);
        }

        public void DecNoteCount(long id, bool isPrivate)
        {
            SourceUser su = obSourceRepository.GetSourceUser(id);
            su.noteCount--;
            if(isPrivate)su.PrivateNoteCount--;
            if (su.PrivateNoteCount == 0 && (su.PrivacyOverride == null || !(bool)su.PrivacyOverride)) su.Privacy = false;

            obSourceRepository.UpdateSourceUser(su);

        }



        internal void DecPrivateNoteCount(long id)
        {
            SourceUser su = obSourceRepository.GetSourceUser(id);
            su.PrivateNoteCount--;
            if (su.PrivateNoteCount == 0 && (su.PrivacyOverride == null || !(bool)su.PrivacyOverride)) su.Privacy = false;

            obSourceRepository.UpdateSourceUser(su);
        }
    }
}