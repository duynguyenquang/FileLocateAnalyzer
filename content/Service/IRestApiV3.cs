using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileLocateAnalyzer.Models;
using Microsoft.AspNetCore.Hosting.Internal;

namespace FileLocateAnalyzer.IService
{
    public interface IRestApiV3
    {
        Task<ICollection<string>> Config(string repo);
    }

    public class RestApiV3 : IRestApiV3
    {
        public Task<ICollection<string>> Config(string repo)
        {
            throw new NotImplementedException();
        }
    }

}
