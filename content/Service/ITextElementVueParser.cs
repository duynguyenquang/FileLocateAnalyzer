using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileLocateAnalyzer.Models;
using HtmlAgilityPack;

namespace FileLocateAnalyzer.IService
{
    public interface ITextElementVueParser
    {
        Task<ICollection<TextElement>> Parse(string htmlContent);
    }

    public class TextElementVueParser : ITextElementVueParser
    {
        private List<TextElement> _textElements;

        public TextElementVueParser()
        {
            _textElements = new List<TextElement>();
        }

        public async Task<ICollection<TextElement>> Parse(string htmlContent)
        {
            var htmlDocument = new HtmlDocument();
            // convert to  html struct then loadHtml
            htmlDocument.LoadHtml($"<html><body>{htmlContent}</body></html>");
            var htmlBody = htmlDocument.DocumentNode.SelectSingleNode("//template");
            var nodes = htmlBody?.SelectNodes("*")?.Where(s =>
                s.Name.ToLower() != "script" && s.Name.ToLower() != "style" && s.Name.ToLower() != "tmodel");
            if (nodes != null && nodes.Any())
                foreach (var node in nodes)
                    await DetectTag(htmlBody.Elements(node.Name));

            return _textElements;
        }
        private async Task DetectTag(IEnumerable<HtmlNode> htmlNodes)
        {
            foreach (var elment in htmlNodes)
                if (elment.ChildNodes.Count > 0)
                {
                    await DetectTag(elment.ChildNodes);
                }
                else
                {
                    if (!string.IsNullOrEmpty(elment.InnerHtml) && !string.IsNullOrWhiteSpace(elment.InnerHtml))
                    {
                        var vLang = elment.ParentNode.Attributes.FirstOrDefault(s => s.Name.Contains("v-lang"));
                        var vLangKey = vLang?.Name.Split('.')?[1];
                        _textElements.Add(new TextElement()
                        {
                            Code = vLangKey?.ToUpper(),
                            InnerText = elment.InnerText,
                            Line = elment.Line
                        });
                    }
                }
        }
    }
}
