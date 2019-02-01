using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileLocateAnalyzer.IService;
using FileLocateAnalyzer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCaching.Internal;

namespace FileLocateAnalyzer.Controllers
{
    [Route("api")]
    public class GitController : Controller
    {

        private readonly IGitService _gitService;
        private readonly ILocalizationAnalizer _localizationAnalizer;

        public GitController(IGitService gitService)
        {
            _gitService = gitService;
        }
        [HttpPost, Route("get-branches")]
        public async Task<IActionResult> Index(string projectName)
        {
            var projectInfor = await _gitService.GetProjectInfo(projectName);
            var product = projectInfor.FirstOrDefault();
            var branche = await _gitService.GetBranchInfor(product.id, product.default_branch);
            var repoTrees = await _gitService.GetRepoTree(product.id, product.default_branch, true);
            var paths = new List<string>();
            foreach (var repo in repoTrees)
            {

                if (repo.name.EndsWith(".cshtml") || repo.name.Contains("resources.js") || repo.name.EndsWith(".vue"))
                {
                    paths.Add(repo.path);
                }
            }

            var allPaths = new List<RepositoryTree>();

            foreach (var path in repoTrees)
            {
                var recursive = path.path != "Vue"; // break case with Path == Vue
                allPaths.AddRange(await _gitService.FindByPath(product.id, product.default_branch, recursive, path.path));
            }
            
            var elementInfors = new List<ElementInfor>();
            //foreach (var path in paths)
            //{
            //    elementInfors.AddRange(await _localizationAnalizer.Analyze(product.id, product.default_branch, path.path));
            //}
            return Json(new { projectInfor });
        }
    }
}
