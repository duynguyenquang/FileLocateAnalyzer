using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using FileLocateAnalyzer.Contants;
using FileLocateAnalyzer.Models;
namespace FileLocateAnalyzer.IService
{
    public interface ILocalizationAnalizer
    {
        Task<ICollection<TextElement>> VueAnalyzeAsync(string content);
        Task<ICollection<TextElement>> CshtmlAnalyze(string content);
        Task<ICollection<ElementInfor>> Analyze(int projectId, string branch, string file);
        Task<Dictionary<string, Dictionary<string, string>>> ResoucesAnalyze(string content);
    }
    public class LocalizationAnalizer : ILocalizationAnalizer
    {
        private readonly ITextElementCshtmlParser _cshtmlParser;
        private readonly ITextElementVueParser _vueParser;
        private readonly IResoucesParser _resoucesParser;
        private readonly IGitService _gitService;

        public LocalizationAnalizer(ITextElementCshtmlParser cshtmlParser, ITextElementVueParser vueParser, IResoucesParser resoucesParser, IGitService gitService)
        {
            _cshtmlParser = cshtmlParser;
            _vueParser = vueParser;
            _resoucesParser = resoucesParser;
            _gitService = gitService;
        }
        public async Task<ICollection<TextElement>> VueAnalyzeAsync(string content)
        {
            var stringContent = new StreamReader(new MemoryStream(Convert.FromBase64String(content))).ReadToEnd();
            if (string.IsNullOrEmpty(stringContent)) return new List<TextElement>();
            return await _vueParser.Parse(stringContent);
        }
        public async Task<ICollection<TextElement>> CshtmlAnalyze(string content)
        {
            var stringContent = new StreamReader(new MemoryStream(Convert.FromBase64String(content))).ReadToEnd();
            if (string.IsNullOrEmpty(stringContent)) return new List<TextElement>();
            return await _cshtmlParser.Parse(content); ;
        }
        public async Task<Dictionary<string, Dictionary<string, string>>> ResoucesAnalyze(string content)
        {
            var stringContent = new StreamReader(new MemoryStream(Convert.FromBase64String(content))).ReadToEnd();
            if (string.IsNullOrEmpty(stringContent)) return new Dictionary<string, Dictionary<string, string>>();
            return await _resoucesParser.Parse(content);
        }

        public async Task<ICollection<ElementInfor>> Analyze(int projectId, string branch, string file)
        {
            if (projectId <= 0 || string.IsNullOrEmpty(branch) || string.IsNullOrEmpty(file)) return null;
            var content = await _gitService.ReadFilePath(projectId, branch, file).ContinueWith(task => task.Result?.content);
            var elementInfors = new Collection<ElementInfor>();
            if (!string.IsNullOrEmpty(content))
            {
                if (file.EndsWith(FileContant.EXTENTION_VUE))
                {
                    elementInfors = (Collection<ElementInfor>)await _vueParser.Parse(content);
                }
                else if (file.EndsWith(FileContant.EXTENTION_CSHTML))
                {
                    elementInfors = (Collection<ElementInfor>)await _cshtmlParser.Parse(content);
                }
                else if (file.Contains(FileContant.RESOUCEFILE))
                {
                   // elementInfors = (Collection<ElementInfor>)await _resoucesParser.Parse(content);
                }
                else
                {
                    return null;
                }
            }

            return elementInfors;
        }
    }
}
