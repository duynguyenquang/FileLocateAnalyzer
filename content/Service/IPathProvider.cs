using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace FileLocateAnalyzer.IService
{
    public interface IPathProvider
    {
	    string MapPath(string path);
	}

	public class PathProvider : IPathProvider
	{
		private IHostingEnvironment _hostingEnvironment;

		public PathProvider(IHostingEnvironment environment)
		{
			_hostingEnvironment = environment;
		}

		public string MapPath(string path)
		{
			var filePath = Path.Combine(_hostingEnvironment.WebRootPath, path);
			return filePath;
		}
	}
}
