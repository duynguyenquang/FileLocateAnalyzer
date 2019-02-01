using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileLocateAnalyzer.Enum;

namespace FileLocateAnalyzer.Models
{
    public class ElementInfor
    {
        public string Code { get; set; }
        public string InnerText { get; set; }
        public int Line { get; set; }
    }

    public class ResponeResult
    {
        public List<ElementInfor> ElementInfors { get; set; }
        public int TotalMissingKey { get; set; }
        public int TotalKeyNotUse { get; set; }
    }
}


