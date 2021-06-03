using System;
using System.Threading;
using System.Threading.Tasks;
using CefBrowserProcess.Browser;
using CefBrowserProcess.Models;
using UnityWebBrowser.Shared;
using Xilium.CefGlue;

namespace CefBrowserProcess.Core
{
    public class CefManager : IDisposable, IBrowser
    {
	    private BrowserProcessCEFClient cefClient;
	    
        public CefManager()
        {
            CefRuntime.Load();
        }

        public void Init(LaunchArguments launchArguments, string[] cefArgs)
        {
            //Do we have a cache or not, if not CEF will run in "incognito" mode.
            string cachePathArgument = null;
            if (launchArguments.CachePath != null)
            	cachePathArgument = launchArguments.CachePath.FullName;

            //Setup the CEF settings
            CefSettings cefSettings = new CefSettings
            {
            	WindowlessRenderingEnabled = true,
            	NoSandbox = true,
            	LogFile = launchArguments.LogPath.FullName,
            	CachePath = cachePathArgument,
            	MultiThreadedMessageLoop = false,
            	LogSeverity = launchArguments.LogSeverity,
            	Locale = "en-US",
            	ExternalMessagePump = false,
#if LINUX
            	//On Linux we need to tell CEF where everything is
            	ResourcesDirPath = System.IO.Path.Combine(Environment.CurrentDirectory),
            	LocalesDirPath = System.IO.Path.Combine(Environment.CurrentDirectory, "locales"),
            	BrowserSubprocessPath = System.IO.Path.Combine(Environment.CurrentDirectory, "cefsimple")
#endif
            };
            
            //Set up CEF args and the CEF app
            CefMainArgs cefMainArgs = new CefMainArgs(cefArgs);
            BrowserProcessCEFApp cefApp = new BrowserProcessCEFApp();
#if WINDOWS
            //Run our sub-processes
            int exitCode = CefRuntime.ExecuteProcess(cefMainArgs, cefApp, IntPtr.Zero);
            if (exitCode != -1)
            	throw new Exception();
#endif

            //Init CEF
            CefRuntime.Initialize(cefMainArgs, cefSettings, cefApp, IntPtr.Zero);

            //Create a CEF window and set it to windowless
            CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
            cefWindowInfo.SetAsWindowless(IntPtr.Zero, false);

            //Create our CEF browser settings
            CefColor backgroundColor = new CefColor(launchArguments.Bca, launchArguments.Bcr, launchArguments.Bcg,
            	launchArguments.Bcb);
            CefBrowserSettings cefBrowserSettings = new CefBrowserSettings
            {
            	BackgroundColor = backgroundColor,
            	JavaScript = launchArguments.JavaScript ? CefState.Enabled : CefState.Disabled,
            	LocalStorage = CefState.Disabled
            };

            Logger.Debug($"CEF starting with these options:" +
                         $"\nJS: {launchArguments.JavaScript}" +
                         $"\nBackgroundColor: {backgroundColor}" +
                         $"\nCache Path: {cachePathArgument}" +
                         $"\nLog Path: {launchArguments.LogPath.FullName}" +
                         $"\nLog Severity: {launchArguments.LogSeverity}");
            Logger.Info("Starting CEF client...");

            //Create cef browser
            try
            {
            	cefClient = new BrowserProcessCEFClient(new CefSize(launchArguments.Width, launchArguments.Height), 
            		new ProxySettings(launchArguments.ProxyUsername, launchArguments.ProxyPassword));
            	CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, launchArguments.InitialUrl);
            }
            catch (Exception ex)
            {
            	Logger.ErrorException(ex, "Something when wrong while creating the CEF client!");
            	throw new Exception();
            }
        }

        public void StartUpdateLoop()
        {
	        CefRuntime.RunMessageLoop();
        }

        public void Dispose()
        {
	        CefRuntime.QuitMessageLoop();
	        cefClient?.Dispose();
	        CefRuntime.Shutdown();
        }

        public Task<byte[]> GetPixels()
        {
	        Logger.Debug("Responding with pixel data...");
	        return Task.FromResult(cefClient.GetPixels());
        }
    }
}