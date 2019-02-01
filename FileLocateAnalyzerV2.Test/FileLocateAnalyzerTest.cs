using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Autofac;
using FileLocateAnalyzer.IService;
using FileLocateAnalyzer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileLocateAnalyzerV2.Test
{
	[TestClass]
	public class GitServiceTest
	{
		private int projectId = 68;
		private string branchName = "dev";
		private string[] extentions = new string[] { "*.vue", "resouces.js", "*.cshtml" };
		private static List<string> paths = new List<string>();
		private bool recursive = true;

		[TestMethod]
		public async Task GetProjectTest()
		{
			using (var c = new InitTest().Init())
			{
				var s = c.Resolve<IGitService>();
				var projects = await s.GetProjectInfo("goquo-engine-malindo");
				int i = 0;
				foreach (var project in projects)
				{
					i++;
					Console.WriteLine($"{i} - {project.id}");
				}
				Assert.IsNotNull(projects);
			}
		}

		[TestMethod]
		public async Task GetAllBranchesTest()
		{
			using (var c = new InitTest().Init())
			{
				var s = c.Resolve<IGitService>();
				var branches = await s.GetBranchs("goquo-engine-malindo");
				int i = 0;
				foreach (var branch in branches)
				{
					i++;
					Console.WriteLine($"{i} - {branch.name}");
				}
				Assert.IsNotNull(branches);
			}
		}

		[TestMethod]
		public async Task GetAllGetRepoTreeTest()
		{
			var sw = new Stopwatch();

			using (var c = new InitTest().Init())
			{
				var s = c.Resolve<IGitService>();
				var repoTrees = await s.GetRepoTree(projectId, branchName, recursive);
				int i = 0;
				foreach (var repo in repoTrees)
				{
					AddToPath(repo.name, repo.path);
					await GetAllPathAsync(s, repo.path);
				}
				var pathVues = paths.Where(e => e.EndsWith(".vue"));
				var pathCshtml = paths.Where(e => e.EndsWith(".cshtml"));
				var resouce = paths.FirstOrDefault(v => v.Contains("resources.js"));

				sw.Start();
				foreach (var item in pathVues)
				{
					i++;
					Console.WriteLine($"{i} - {item} ");
					await LocaliztionVueTest(item);
				}
				foreach (var path in pathCshtml)
				{
					i++;
					Console.WriteLine($"{i} - {path} ");
					await LocaliztionCshtmlTest(path);
				}
				await LocaliztionResouceTest();
				sw.Stop();
				ConsoleLog(sw.ElapsedMilliseconds);

			}
			Assert.IsNotNull(paths);
		}

		private async Task GetAllPathAsync(IGitService gitService, string repoPath)
		{
			recursive = repoPath != "Vue"; // break case with Path == Vue
			var allPaths = await gitService.FindByPath(projectId, branchName, recursive, repoPath);
			foreach (var item in allPaths)
			{
				AddToPath(item.name, item.path);
				//await GetAllPathAsync(gitService, item.path);
			}
		}

		private void AddToPath(string name, string path)
		{
			if (name.EndsWith(".cshtml") || name.Contains("resources.js") || name.EndsWith(".vue"))
			{
				paths.Add(path);
			}
		}

		[TestMethod]
		public async Task GetRepoTreePathTest()
		{
			int i = 0;
			using (var c = new InitTest().Init())
			{
				var s = c.Resolve<IGitService>();
				var repoTrees = await s.FindByPath(projectId, branchName, recursive, "Vue/Components");
				foreach (var repo in repoTrees)
				{
					AddToPath(repo.name, repo.path);
					//GetAllPathAsync(repo.path);
				}
			}
			foreach (var item in paths)
			{
				i++;
				Console.WriteLine($"{i} - {item} ");
			}
		}

		[TestMethod]
		public async Task GetBranchInforTest()
		{
			using (var c = new InitTest().Init())
			{
				var s = c.Resolve<IGitService>();
				var branch = await s.GetBranchInfor(projectId, branchName);
				Console.WriteLine(branch.name);
				Assert.IsNotNull(branch);
			}
		}

		[TestMethod]
		public async Task GetContentFileTest()
		{
			using (var c = new InitTest().Init())
			{
				var sw = new Stopwatch();
				var s = c.Resolve<IGitService>();
				var file = await s.ReadFilePath(projectId, "Vue/resources.js", branchName);
				Byte[] bytes = Convert.FromBase64String(file.content);
				var stream = new StreamReader(new MemoryStream(bytes));
				Console.WriteLine($"{stream.ReadToEnd()}");
				Assert.IsNotNull(bytes);
				sw.Stop();
				ConsoleLog(sw.ElapsedMilliseconds);
			}

		}

		[TestMethod]
		public async Task LocaliztionVueTest(string path)
		{
			using (var c = new InitTest().Init())
			{
				var s = c.Resolve<IGitService>();
				var file = await s.ReadFilePath(projectId, path, branchName);
				//Byte[] bytes = Convert.FromBase64String(file.content);
				//var stream = new StreamReader(new MemoryStream(bytes));
				//var content = stream.ReadToEnd();
				//Console.WriteLine($"{content}");
				var localizationAnalizer = c.Resolve<ILocalizationAnalizer>();
				var values = await localizationAnalizer.VueAnalyzeAsync(file.content);
				int i = 0;
				foreach (var value in values)
				{
					i++;
					Console.WriteLine($"{i} - {value.Line} - {value.Code} - {value.InnerText}");
				}
			}
		}

		[TestMethod]
		public async Task LocaliztionCshtmlTest(string path)
		{
			using (var c = new InitTest().Init())
			{
				var s = c.Resolve<IGitService>();
				var file = await s.ReadFilePath(projectId, path, branchName);
				Byte[] bytes = Convert.FromBase64String(file.content);
				var stream = new StreamReader(new MemoryStream(bytes));
				var content = stream.ReadToEnd();
				//Console.WriteLine($"{content}");
				var localizationAnalizer = c.Resolve<ILocalizationAnalizer>();
				var values = await localizationAnalizer.CshtmlAnalyze(content);
				int i = 0;
				foreach (var value in values)
				{
					i++;
					Console.WriteLine($"{i} - {value.Line} - {value.Code} - {value.InnerText}");
				}
				Assert.IsNotNull(bytes);
			}
		}

		[TestMethod]
		public async Task LocaliztionResouceTest()
		{
			var sw = new Stopwatch();
			using (var c = new InitTest().Init())
			{
				var s = c.Resolve<IGitService>();
				var file = await s.ReadFilePath(projectId, "Vue/resources.js", branchName);
				Byte[] bytes = Convert.FromBase64String(file.content);
				var stream = new StreamReader(new MemoryStream(bytes));
				var content = stream.ReadToEnd();
				var localizationAnalizer = c.Resolve<ILocalizationAnalizer>();
				var values = await localizationAnalizer.ResoucesAnalyze(content);
				int i = 0;
				foreach (var value in values)
				{
					i++;
					Console.WriteLine($"{i} - {value.Key} - {string.Join(",", value.Value.Select(w => w.Value))}");
				}
				Assert.IsNotNull(bytes);
			}
			sw.Stop();
			ConsoleLog(sw.ElapsedMilliseconds);
		}

		[TestMethod]
		public async Task TestAll()	
		{
			using (var c = new InitTest().Init())
			{
				var sw1 = new Stopwatch();
				sw1.Start();
				var s = c.Resolve<IGitService>();
				var repoTrees = await s.GetRepoTree(projectId, branchName, recursive);
				int i = 0;

				
				//Parallel.ForEach(actions, a => a.Invoke());
				foreach (var repo in repoTrees)
				{
					AddToPath(repo.name, repo.path);
					recursive = repo.path != "Vue"; // break case with Path == Vue
					var allPaths = await s.FindByPath(projectId, branchName, recursive, repo.path);
					foreach (var item in allPaths)
					{
						AddToPath(item.name, item.path);
					}
				}
				sw1.Stop();
				ConsoleLog(sw1.ElapsedMilliseconds);
				var pathVues = paths.Where(e => e.EndsWith(".vue"));
				var pathCshtml = paths.Where(e => e.EndsWith(".cshtml"));
				var pathResouce = paths.FirstOrDefault(v => v.Contains("resources.js"));
				var cshtmlTextElement = new List<TextElement>();
				var vuesTextElement = new List<TextElement>();
				var localizationAnalizer = c.Resolve<ILocalizationAnalizer>();
				sw1.Restart();
				foreach (var path in pathVues)
				{
					var fileVue = s.ReadFilePath(projectId, path, branchName).Result;
					var results = localizationAnalizer
						.VueAnalyzeAsync(fileVue?.content)
						.Result;
					if (results != null && results.Any())
						vuesTextElement.AddRange(results);
				}
				foreach (var path in pathCshtml)
				{
					var fileVue = s.ReadFilePath(projectId, path, branchName).Result;
					var results = localizationAnalizer
						.CshtmlAnalyze(fileVue?.content)
						.Result;
					if (results != null && results.Any())
						cshtmlTextElement.AddRange(results);
				}
				var file = s.ReadFilePath(projectId, pathResouce, branchName).Result;
				var values = localizationAnalizer.ResoucesAnalyze(file?.content);

				sw1.Stop();
				ConsoleLog(sw1.ElapsedMilliseconds);

			}
			Assert.IsNotNull(paths);
		}

		private void ConsoleLog(long miniSecond)
		{
			var ts = TimeSpan.FromMilliseconds(miniSecond);
			Console.WriteLine("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
				ts.Hours,
				ts.Minutes,
				ts.Seconds,
				ts.Milliseconds);
		}

	}
}
