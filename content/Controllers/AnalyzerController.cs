using System.Threading.Tasks;
using FileLocateAnalyzer.IService;
using Microsoft.AspNetCore.Mvc;

namespace FileLocateAnalyzer.Controllers
{
    [Route("api")]
    public class AnalyzerController : Controller
    {
        public AnalyzerController()
        {

        }
        [HttpPost, Route("analyzer")]
        public async Task<IActionResult> Index(string gitUrl)
        {
            return Json(new { });
        }
    }
}
