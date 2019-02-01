using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FileLocateAnalyzer.IService
{
    public interface IResoucesParser
    {
        Task<Dictionary<string, Dictionary<string, string>>> Parse(string htmlContent);
    }

    public class ResoucesParser : IResoucesParser
    {
        private const string EXPORT_DEFAULT = "export default";
        public async Task<Dictionary<string, Dictionary<string, string>>> Parse(string jsonContent)
        {
            jsonContent = jsonContent.Replace(EXPORT_DEFAULT, "");
            return  JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonContent);
        }
    }
}
