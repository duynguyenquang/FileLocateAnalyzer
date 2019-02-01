using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileLocateAnalyzer.Models;

namespace FileLocateAnalyzer.IService
{
    public interface ILocalizationAnalizer
    {
        Task<ElementInfor> Analyze(int projectId, string branch, string file);
    }
}
