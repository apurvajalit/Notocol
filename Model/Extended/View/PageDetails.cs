using System.Collections.Generic;

namespace Model.Extended.View
{
    public class PageDetails
    {
        public long Id { get; set; }
        public long sourceUserID { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string faviconURL { get; set; }
        public string thumbnailImageUrl { get; set; }
        public string thumbnailText { get; set; }
        public List<string> tags { get; set; }
        public List<NoteDisplay> notes { get; set; }
        public List<string> users { get; set; }
        public bool ownPage { get; set; }
    }
}
