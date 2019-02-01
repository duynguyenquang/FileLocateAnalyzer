using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using FileLocateAnalyzer.Models;
using FileLocateAnalyzer.IService;
using GoQuoEngine.Core.Configurations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileLocateAnalyzerV2.Test
{
	[TestClass]
	public class InitTest
	{
		public IContainer Init()
		{
			var builder = new ContainerBuilder();
			builder.RegisterType<GitService>().As<IGitService>().SingleInstance();
			
			builder.RegisterType<TextElementCshtmlParser>().As<ITextElementCshtmlParser>().SingleInstance();
			builder.RegisterType<TextElementVueParser>().As<ITextElementVueParser>().SingleInstance();
			builder.RegisterType<LocalizationAnalizer>().As<ILocalizationAnalizer>().SingleInstance();
			builder.RegisterType<ResoucesParser>().As<IResoucesParser>().SingleInstance();
			builder.Configure(new GitConfiguration
			{
				//Url = "https://git.goquo.vn/",
				//PrivateToken = "jPSL5n-qdNt_ajfKfZr-"
			});
			return builder.Build();
		}
	}
}
