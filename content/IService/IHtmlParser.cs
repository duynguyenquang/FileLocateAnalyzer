using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileLocateAnalyzer.Models;

namespace FileLocateAnalyzer.IService
{
    public interface IHtmlParser
    {
        Task<ICollection<ElementInfor>> CshtmlParse(string content);
        Task<ICollection<ElementInfor>> VueParse(string content);
    }
}
