using System;
using CefBrowserProcess.Models;
using Xilium.CefGlue;

namespace CefBrowserProcess.Core
{
	/// <summary>
	///		Main class responsible for the app
	///		<para>
	///			This class handles managing CEF and talks back to the client using ZMQ
	///		</para>
	/// </summary>
	public class CefBrowserProcess : IDisposable
	{
		private readonly CefManager cefManager;
		private readonly CommunicationsManager comsManager;
		
		///  <summary>
		/// 		Creates a new <see cref="CefBrowserProcess"/> instance
		///  </summary>
		///  <param name="launchArguments"></param>
		///  <param name="cefArgs"></param>
		///  <exception cref="Exception"></exception>
		public CefBrowserProcess(LaunchArguments launchArguments, string[] cefArgs)
		{
			//Is debug log enabled or not
			Logger.DebugLog = launchArguments.Debug;

			//Setup CEF
			cefManager = new CefManager();
			cefManager.Init(launchArguments, cefArgs);
			
			//Setup the coms
			comsManager = new CommunicationsManager(launchArguments, cefManager);
			comsManager.StartListening();
			
			cefManager.StartUpdateLoop();
		}

		#region Destroy

		public void Dispose()
		{
			comsManager.Dispose();
			cefManager.Dispose();
			CefRuntime.Shutdown();
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}