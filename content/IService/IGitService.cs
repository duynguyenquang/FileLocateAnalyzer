using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FileLocateAnalyzer.Models;

namespace FileLocateAnalyzer.IService
{
    public interface IGitService
    {
        Task<ICollection<Project>> GetProjects();
        Task<ICollection<Branch>> GetBranches(int projectId);
        Task<Collection<string>> GetPaths(int projectId, string branch);
        Task<string> ReadPath(int projectId, string branch, string path);
    }
}
