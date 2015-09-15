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

        internal SourceUser AddSourceUser(SourceUser sourceUser)
        {
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
            if (sourceData.sourceUserID > 0)
            {
                sourceUser = obSourceRepository.GetSourceUser(sourceData.sourceUserID);
                //TODO SHould nver happen
                if (sourceUser == null) return null;

                sourceUser.FolderID = sourceData.folder;
                sourceUser.Summary = sourceData.summary;
                sourceUser.Privacy = sourceData.privacy;
                tagHelper.UpdateSourceTags(sourceUser, sourceData.tags);
                sourceUser = UpdateSourceUser(sourceUser);
            }
            else
            {
                if (sourceData.sourceID <= 0)
                {
                    Source source = new Source();
                    source.title = sourceData.title;
                    source.faviconURL = sourceData.faviconUrl;
                    source.url = sourceData.url;
                    source = AddSource(sourceData.uri, source);
                    if (source == null || source.ID <= 0) return null;
                    sourceData.sourceID = source.ID;
                }
                sourceUser = new SourceUser();
                sourceUser.SourceID = sourceData.sourceID;
                sourceUser.FolderID = sourceData.folder;
                sourceUser.Summary = sourceData.summary;
                sourceUser.Privacy = sourceData.privacy;
                sourceUser.UserID = userID;
                sourceUser.noteCount = 0;
                sourceUser = AddSourceUser(sourceUser);
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
                sourceDataForExtension.folder = (sourceUser.FolderID != null) ? (int)sourceUser.FolderID : 0;
                sourceDataForExtension.tags = new TagHelper().GetSourceTags(sourceUser.ID);
            }

            return sourceDataForExtension;
        }
        
        //public Source GetSource(string url, long userID)
        //{
        //    return obSourceRepository.GetSourceFromSourceURI(url, userID);
        //}
        //public bool UpdateSource(Source source)
        //{
        //    return obSourceRepository.UpdateSource(source);
                
        //}
        //public long AddOrUpdateSourceFromExtension(long userID, SourceDataForExtension sourceData)
        //{
        //    if (userID <= 0) return 0;
        //    long sourceID = sourceData.sourceID;
        //    SourceRepository objSourceRepository = new SourceRepository();
        //    Source objSource;
        //    if (sourceID > 0)
        //    { 
        //        //We assume extension would set the sourceID if it has already been added
        //        objSource = objSourceRepository.GetSource(sourceID);
        //        if (objSource == null || objSource.UserID != userID) return 0;

        //    }else if((objSource = objSourceRepository.GetSourceFromSourceURI(sourceData.url, userID)) == null){
                
        //        objSource = new Source();
        //        objSource.UserID = userID;
        //        objSource.SourceURI = sourceData.url;
                
        //    }

        //    objSource.Title = sourceData.title;
        //    objSource.Summary = sourceData.summary;
        //    objSource.FaviconURL = sourceData.faviconUrl;
        //    objSource.URN = sourceData.urn;

        //    if (sourceData.folder > 0 && objSource.FolderID != sourceData.folder)
        //    {
        //        //Set the folder here after checking whether it is created or not
        //    }

        //    bool updateTags = false;
        //    if (objSource.ID == 0)
        //    {
        //        if (objSourceRepository.AddSource(objSource) != null){
        //            sourceID = objSource.ID;
        //            if((sourceData.tags!= null) && (sourceData.tags.Count() > 0)) updateTags = true;
        //        }
        //    }
        //    else {
        //        if (objSourceRepository.UpdateSource(objSource)) {
        //            sourceID = objSource.ID;
        //            updateTags = true;
        //        }
        //    }

        //    if (updateTags) { 
        //        TagHelper tagHelper = new TagHelper();
        //        tagHelper.UpdateSourceTags(objSource, sourceData.tags);
        //    }
        //    return sourceID;
        //}
        //public SourceDataForExtension GetSourceExtensionData(string pageURL, long userID)
        //{
        //    SourceDataForExtension sourceDataForExtension = new SourceDataForExtension();
        //    Source source = new SourceHelper().GetSource(pageURL, userID);
        //    if (source != null)
        //    {
        //        TagHelper tagHelper = new TagHelper();

        //        sourceDataForExtension.urn = source.URN;
        //        sourceDataForExtension.url = source.SourceURI;
        //        sourceDataForExtension.summary = source.Summary;
        //        sourceDataForExtension.title = source.Title;
        //        sourceDataForExtension.sourceID = source.ID;
        //        sourceDataForExtension.faviconUrl = source.FaviconURL;
        //        sourceDataForExtension.folder = (source.FolderID != null) ? (long)source.FolderID : 0;
        //        sourceDataForExtension.noteCount = source.noteCount;
        //        sourceDataForExtension.tags = tagHelper.GetSourceTags(source.ID);
        //    }
            
        //    return sourceDataForExtension;
        //}
        
    }
}