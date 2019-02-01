using System.Collections.Generic;

namespace FileLocateAnalyzer.Models
{
    public class TextElement
    {
        public string Code { get; set; }
        public string InnerText { get; set; }
        public int Line { get; set; }
        public IDictionary<string, string> Attributes { get; set; }
    }

}
