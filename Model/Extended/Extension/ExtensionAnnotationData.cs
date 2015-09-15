using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Extended.Extension
{

    public class Selector
    {
        public string type { get; set; }
        public string startContainer { get; set; }
        public string endContainer { get; set; }
        public int startOffset { get; set; }
        public string endOffset { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public string prefix { get; set; }
        public string exact { get; set; }
        public string suffix { get; set; }
        public string value { get; set; }
    }
    public class Position
    {
        public int top { get; set; }
        public int height { get; set; }
    }
    public class Target
    {
        public string source { get; set; }
        public Position pos { get; set; }
        public Selector[] selector { get; set; }
    }
    public class ExtensionAnnotationData
    {
        public string updated { get; set; }
        public Target[] target { get; set; }
        public string created { get; set; }
        public string text { get; set; }
        public string[] tags { get; set; }
        public string uri { get; set; }
        public string user { get; set; }
        public Object document { get; set; }
        public string consumer { get; set; }
        public long id { get; set; }
        public Object permissions { get; set; }
    }
}
