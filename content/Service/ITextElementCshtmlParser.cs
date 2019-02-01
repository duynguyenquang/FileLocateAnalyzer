using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileLocateAnalyzer.Models;

namespace FileLocateAnalyzer.IService
{
    public interface ITextElementCshtmlParser
    {
        Task<ICollection<TextElement>> Parse(string htmlContent);
    }

    public class TextElementCshtmlParser : ITextElementCshtmlParser
    {
        private List<TextElement> _textElements;

        public TextElementCshtmlParser()
        {
            _textElements = new List<TextElement>();
        }
        public async Task<ICollection<TextElement>> Parse(string htmlContent)
        {
            var pattern = "(?<=T\\(\")[^\"]+(?=\")";
            var matches = Regex.Matches(htmlContent, pattern);
            if (matches.Count > 0)
                foreach (Match m in matches)
                    _textElements.Add(new TextElement
                    {
                        Code = m.Value.ToUpper(),
                        InnerText = m.Value,
                    });
            return _textElements;
        }
    }
}
