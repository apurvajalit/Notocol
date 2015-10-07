using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended
{
    public class NoteData
    {
        public string NoteText  { get; set; }
        public string QuotedText { get; set; }
        public long id { get; set; }
        public string username { get; set; }
        public DateTime updated { get; set; }
        public string[] tags { get; set; }
        public string  pageURL { get; set; }
        public string pageTitle { get; set; }
    }
}
