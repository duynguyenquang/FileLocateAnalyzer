using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileLocateAnalyzer.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace FileLocateAnalyzer.Service
{

    public interface ICheckingLangHelper
    {
        Task<ResponeResult> Run();
    }

    public class CheckingLangHelper : ICheckingLangHelper
    {
        private List<Language> _vueLanguages = new List<Language>();
        private List<Language> _cshtmlLanguages = new List<Language>();
        private List<LangResouce> _resoucesLanguages = new List<LangResouce>();
        private Dictionary<string, Dictionary<string, string>> _dictJsonOutput = new Dictionary<string, Dictionary<string, string>>();
        private const string EXPORT_DEFAULT = "export default";


        public async Task<ResponeResult> Run()
        {
            //var path = @"D:\CloneGit";
            var pathMock = @"D:\Work\CitiLink\malindo";
            var responeResult = new ResponeResult();

            var files = Directory.GetFiles(pathMock, "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".cshtml")
                            || s.Contains("resources.js")
                            || s.EndsWith(".vue"));

            foreach (var file in files)
                if (file.EndsWith(".vue") || file.EndsWith(".cshtml"))
                {
                    var allText = File.ReadAllText(file);
                    if (!string.IsNullOrEmpty(allText))
                        if (file.EndsWith(".vue"))
                        {
                            var body = "<html><head></head><body>Hello, this is {0}...</body></html>";
                            var html = allText.Contains("<body>") ? allText : string.Format(body, allText);
                            var htmlDocument = new HtmlDocument();
                            htmlDocument.LoadHtml(html);
                            var htmlBody = htmlDocument.DocumentNode.SelectSingleNode("//template");
                            var nodes = htmlBody?.SelectNodes("*")?.Where(s =>
                                s.Name.ToLower() != "script" && s.Name.ToLower() != "style" && s.Name.ToLower() != "tmodel");
                            if (nodes != null && nodes.Any())
                                foreach (var node in nodes)
                                    Genareate(file, htmlBody.Elements(node.Name));
                        }
                        else
                        {
                            var pattern = "(?<=T\\(\")[^\"]+(?=\")";
                            var matches = Regex.Matches(allText, pattern);
                            if (matches.Count > 0)
                                foreach (Match m in matches)
                                    _cshtmlLanguages.Add(new Language
                                    {
                                        Key = m.Value.ToUpper(),
                                        Source = file,
                                        Line = m.Index
                                    });
                        }
                }
                else if (file.Contains("resources.js"))
                {
                    var jsonContent = File.ReadAllText(file);
                    jsonContent = jsonContent.Replace(EXPORT_DEFAULT, "");
                    _dictJsonOutput = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonContent);
                    if (_dictJsonOutput != null && _dictJsonOutput.Any())
                        foreach (var item in _dictJsonOutput)
                        {
                            var isUse = false;
                            if (_vueLanguages.Count(s => s.Key == item.Key) > 0 || _cshtmlLanguages.Count(s => s.Key == item.Key) > 0)
                                isUse = true;
                            else
                                responeResult.KeyExcess++;
                            _resoucesLanguages.Add(new LangResouce
                            {
                                Key = item.Key,
                                ValueKey =
                                    item.Value.FirstOrDefault(s => s.Key.ToLower() == "en-us").Value ?? item.Value.FirstOrDefault().Value,
                                IsUse = isUse
                            });
                        }
                }

            var vueKeyMiss = _vueLanguages.Where(s => !_dictJsonOutput.Keys.Contains(s.Key)).ToList();
            var cshtmlKeyMiss = _cshtmlLanguages.Where(s => !_dictJsonOutput.Keys.Contains(s.Key)).ToList();
            responeResult.CshtmlMissKeys = cshtmlKeyMiss;
            responeResult.VueMissKeys = vueKeyMiss;
            responeResult.KeyMissing = vueKeyMiss.Count + cshtmlKeyMiss.Count;
            responeResult.ResouceLangs = _resoucesLanguages;
            return responeResult;
        }

        private void Genareate(string file, IEnumerable<HtmlNode> htmlNodes)
        {
            foreach (var elment in htmlNodes)
                if (elment.ChildNodes.Count > 0)
                {
                    Genareate(file, elment.ChildNodes);
                }
                else
                {
                    if (!string.IsNullOrEmpty(elment.InnerHtml) && !string.IsNullOrWhiteSpace(elment.InnerHtml))
                    {
                        var vLang = elment.ParentNode.Attributes.FirstOrDefault(s => s.Name.Contains("v-lang"));
                        var vLangKey = vLang?.Name.Split('.')?[1];
                        if (!string.IsNullOrEmpty(vLangKey))
                        {
                            _vueLanguages.Add(new Language()
                            {
                                Key = vLangKey.ToUpper(),
                                Source = file,
                                Line = elment.Line
                            });
                        }
                    }
                }
        }
    }
}
