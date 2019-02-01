using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FileLocateAnalyzer.Contants;
using FileLocateAnalyzer.IService;
using FileLocateAnalyzer.Models;

namespace FileLocateAnalyzer.Service
{
    public class LocalizationAnalizer : ILocalizationAnalizer
    {

        private readonly IGitService _gitService;

        private readonly IHtmlParser _htmlParser;

        private readonly IResourceParser _resourceParser;

        public LocalizationAnalizer(IGitService gitService, IHtmlParser htmlParser, IResourceParser resourceParser)
        {
            _gitService = gitService;
            _htmlParser = htmlParser;
            _resourceParser = resourceParser;
        }

        public async Task<ElementInfor> Analyze(int projectId, string branch, string file)
        {
            if (projectId <= 0 || string.IsNullOrEmpty(branch) || string.IsNullOrEmpty(file)) return null;
            var content = await _gitService.ReadPath(projectId, branch, file);
            var elementInfors = new Collection<ElementInfor>();
            if (!string.IsNullOrEmpty(content))
            {
                if (file.EndsWith(FileContant.EXTENTION_VUE))
                {
                    elementInfors = (Collection<ElementInfor>) await _htmlParser.VueParse(content);
                }
                else if (file.EndsWith(FileContant.EXTENTION_CSHTML))
                {
                    elementInfors = (Collection<ElementInfor>)await _htmlParser.CshtmlParse(content);
                }
                else if (file.Contains(FileContant.RESOUCEFILE))
                {
                    await _resourceParser.Parse(content);
                }
                else
                {
                    return null;
                }
            }

            return new ElementInfor();
        }
    }
}
