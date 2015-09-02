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
namespace Business
{
    public class SourceHelper
    {
        static SourceRepository obSourceRepository = new SourceRepository();

        public IList<SourceData> GetSourceItems(string keywordFilter, IList<long> tagIDs , long userID, bool onlyUserSource = true){

            IList<SourceData> retList = new List<SourceData>();
            if(onlyUserSource)
                return obSourceRepository.Search(keywordFilter, tagIDs, userID);
            else { 
                
                return obSourceRepository.Search(keywordFilter, tagIDs);
            }
        }

        public Source GetSource(long sourceID)
        {
            return obSourceRepository.GetSource(sourceID);
        }
        public Source GetSource(string url, long userID)
        {
            return obSourceRepository.GetSourceFromSourceURI(url, userID);
        }

        public bool UpdateSource(Source source)
        {
            return obSourceRepository.UpdateSource(source);
                
        }
        public long AddOrUpdateSourceFromExtension(long userID, SourceDataForExtension sourceData)
        {
            if (userID <= 0) return 0;
            long sourceID = sourceData.sourceID;
            SourceRepository objSourceRepository = new SourceRepository();
            Source objSource;
            if (sourceID > 0)
            { 
                //We assume extension would set the sourceID if it has already been added
                objSource = objSourceRepository.GetSource(sourceID);
                if (objSource == null || objSource.UserID != userID) return 0;

            }else if((objSource = objSourceRepository.GetSourceFromSourceURI(sourceData.url, userID)) == null){
                
                objSource = new Source();
                objSource.UserID = userID;
                objSource.SourceURI = sourceData.url;
                
            }

            objSource.Title = sourceData.title;
            objSource.Summary = sourceData.summary;
            objSource.FaviconURL = sourceData.faviconUrl;
            objSource.URN = sourceData.urn;
            if (sourceData.folder > 0 && objSource.FolderID != sourceData.folder)
            {
                //Set the folder here after checking whether it is created or not
            }

            if (objSource.ID == 0)
            {
                if (objSourceRepository.AddSource(objSource) != null)
                    sourceID = objSource.ID;
            }
            else {
                if (objSourceRepository.UpdateSource(objSource)) sourceID = objSource.ID;
            }

            if ((sourceID > 0) && (sourceData.tags!= null) && (sourceData.tags.Count() > 0)) { 
                TagHelper tagHelper = new TagHelper();
                tagHelper.UpdateSourceTags(objSource, sourceData.tags);
            }
            return sourceID;
        }
        
        public SourceDataForExtension GetSourceExtensionData(string pageURL, long userID)
        {
            SourceDataForExtension sourceDataForExtension = new SourceDataForExtension();
            Source source = new SourceHelper().GetSource(pageURL, userID);
            if (source != null)
            {
                TagHelper tagHelper = new TagHelper();

                sourceDataForExtension.urn = source.URN;
                sourceDataForExtension.url = source.SourceURI;
                sourceDataForExtension.summary = source.Summary;
                sourceDataForExtension.title = source.Title;
                sourceDataForExtension.sourceID = source.ID;
                sourceDataForExtension.faviconUrl = source.FaviconURL;
                sourceDataForExtension.folder = (source.FolderID != null) ? (long)source.FolderID : 0;
                sourceDataForExtension.noteCount = source.noteCount;
                sourceDataForExtension.tags = tagHelper.GetSourceTags(source.ID);
            }
            
            return sourceDataForExtension;
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
            

            if(pageImageInfo[max_area_index].score > 0) pageImageInfo[max_area_index].score += 3;
            
            pageImageInfo = pageImageInfo.Where(pageImage =>
                                                    pageImage.score > 0).ToArray<PageImageInfo>();

            Array.Sort(pageImageInfo, new ImageInfoComparer());

            IList<string> imageLinks = new List<string>();

            imageLinks = (from imageLink in pageImageInfo
                          select imageLink.url).ToList();

            if (imageLinks.Count > 0) return imageLinks[0];

            return null;
        }


        private string GetThumbNailText(string pageURL, string[] pageText, int maxThumbnailTextLength)
        {
            string pageThumbnailText = "";
            int numCharsLeft = maxThumbnailTextLength;
            foreach (string text in pageText)
            {
                if (text.Length > 0) { 
                    pageThumbnailText = string.Concat(pageThumbnailText, text.Trim().Substring(0, numCharsLeft < text.Length ? numCharsLeft : text.Length - 1 ), System.Environment.NewLine);
                    numCharsLeft = maxThumbnailTextLength - pageThumbnailText.Length;
                    if (numCharsLeft <= 0) break;
                }
                
            }

            return pageThumbnailText;
        }

        

        public void SetPageThumbNailData(long userID, ThumbnailDataFromSource thumbnailData)
        {
            SourceRepository sourceRepository = new SourceRepository();
            Source source = sourceRepository.GetSourceFromSourceURI(thumbnailData.pageURI, userID);
            if (source == null || source.ID <= 0) return;

            int MaxThumbnailTextLength = 500;

            
            PageImageInfo[] pageImageInfo = JsonConvert.DeserializeObject<PageImageInfo[]>(thumbnailData.imageObjects);

            string[] pageText = JsonConvert.DeserializeObject<string[]>(thumbnailData.textData);

            string thumbnailImageURL = GetThumbNailImage(thumbnailData.pageURI, pageImageInfo);
                
            if (thumbnailImageURL != null) MaxThumbnailTextLength = 200;

            string thumbnailImageText = GetThumbNailText(thumbnailData.pageURI, pageText, MaxThumbnailTextLength);

            source.thumbnailImageUrl = thumbnailImageURL;
            source.thumbnailText = thumbnailImageText;

            sourceRepository.UpdateSource(source);

            return;
        }



        internal Source Add(Source source)
        {
            SourceRepository sourceRepository = new SourceRepository();
            
            return sourceRepository.AddSource(source);

        }

        internal Source GetSourceFromURN(string sourceURN, int userID)
        {
            return obSourceRepository.GetSourceFromSourceURN(sourceURN, userID);
        }
    }
}