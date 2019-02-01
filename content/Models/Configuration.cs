using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileLocateAnalyzer.Models
{
    public class GitConfiguration
    {
        public string Url => "https://git.goquo.vn";

        public string PrivateToken => "jPSL5n-qdNt_ajfKfZr-";

        public Dictionary<string, string> ProjectTokens { get; set; }


        //public string Url { get; set; }

        //public string PrivateToken { get; set; }

        //public Dictionary<string, string> ProjectTokens { get; set; }
    }
}
