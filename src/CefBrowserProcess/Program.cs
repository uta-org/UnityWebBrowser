﻿using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using CefBrowserProcess.Core;
using CefBrowserProcess.Models;
using Xilium.CefGlue;

namespace CefBrowserProcess
{
	/// <summary>
	///		Main class for this program
	/// </summary>
	public static class Program
	{
		/// <summary>
		///		Entry point
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public static int Main(string[] args)
		{
			RootCommand rootCommand = new RootCommand
			{
				//We got a lot of arguments
				new Option<string>("-initial-url",
					() => "https://voltstro.dev",
					"The initial URL"),
				new Option<int>("-width",
					() => 1920,
					"The width of the window"),
				new Option<int>("-height",
					() => 1080,
					"The height of the window"),
				new Option<bool>("-javascript",
					() => true,
					"Enable or disable javascript"),
				new Option<byte>("-bcr",
					() => 255,
					"Background color (red)"),
				new Option<byte>("-bcg",
					() => 255,
					"Background color (green)"),
				new Option<byte>("-bcb",
					() => 255,
					"Background color (blue)"),
				new Option<byte>("-bca",
					() => 255,
					"Background color (alpha)"),
				new Option<FileInfo>("-cache-path", 
					() => null,
					"The path to the cache (null for no cache)"),
				new Option<string>("-proxy-username",
					() => null,
					"The username to use in proxy auth"),
				new Option<string>("-proxy-password",
					() => null, 
					"The proxy auth password"),
				new Option<int>("-port",
					() => 5555,
					"IPC port"),
				new Option<bool>("-debug", 
					() => false,
					"Use debug logging?"),
				new Option<FileInfo>("-log-path", 
					() => new FileInfo("cef.log"),
					"The path to where the CEF log will be"),
				new Option<CefLogSeverity>("-log-severity", 
					() => CefLogSeverity.Default,
					"The path to where the CEF log will be"),
			};
			rootCommand.Description = "Process for windowless CEF rendering.";
			//CEF launches the same process multiple times for its sub-process and passes args to them, so we need to ignore unknown tokens
			rootCommand.TreatUnmatchedTokensAsErrors = false;
			rootCommand.Handler = CommandHandler.Create<LaunchArguments>(parsedArgs =>
			{
				Core.CefBrowserProcess browserProcess = new Core.CefBrowserProcess(parsedArgs, args);
				
				browserProcess.Dispose();
			});
			//Invoke the command line parser and start the handler (the stuff above)
			return rootCommand.InvokeAsync(args).Result;
		}
	}
}