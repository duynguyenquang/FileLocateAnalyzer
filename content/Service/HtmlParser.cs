using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileLocateAnalyzer.IService;
using FileLocateAnalyzer.Models;

namespace FileLocateAnalyzer.IService
{
    public class HtmlParser : IHtmlParser
    {
        public Task<ICollection<ElementInfor>> CshtmlParse(string content)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<ElementInfor>> VueParse(string content)
        {
            throw new NotImplementedException();
        }
    }
}
