using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended.Extension
{
    public class SourceTagData
    {
        public string text { get; set; }
        public long id { get; set; }
    }
    public class SourceDataForExtension
    {
        public string url { get; set; }
        public string uri { get; set; }
        public string title { get; set; }
        public string summary { get; set; }
        public string faviconUrl { get; set; }
        public List<SourceTagData> tags { get; set; }
        public FolderDataFromExtension folderData { get; set; }
        public long sourceID { get; set; }
        public long sourceUserID { get; set; }
        public int noteCount { get; set; }
        public bool privacy { get; set; }
    }
}
