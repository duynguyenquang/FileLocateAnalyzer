using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileLocateAnalyzer.IService
{
    public interface IResourceParser
    {
        Task<Dictionary<string, Dictionary<string, string>>> Parse(string content);
    }
}
