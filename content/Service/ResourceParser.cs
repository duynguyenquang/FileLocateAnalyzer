using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileLocateAnalyzer.IService;

namespace FileLocateAnalyzer.IService
{
    public class ResourceParser : IResourceParser
    {
        public Task<Dictionary<string, Dictionary<string, string>>> Parse(string content)
        {
            throw new NotImplementedException();
        }
    }
}
