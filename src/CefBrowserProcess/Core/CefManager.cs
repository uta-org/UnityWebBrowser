using System;
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

        #region Browser RPC

        public Task<byte[]> GetPixels()
        {
	        Logger.Debug("Responding with pixel data...");
	        return Task.FromResult(cefClient.GetPixels());
        }

        public Task SendKeyDownEvent(int keyCode)
        {
	        cefClient.KeyEvent(new CefKeyEvent
	        {
		        WindowsKeyCode = keyCode,
		        EventType = CefKeyEventType.KeyDown
	        });
	        return Task.CompletedTask;
        }

        public Task SendKeyUpEvent(int keyCode)
        {
	        cefClient.KeyEvent(new CefKeyEvent
	        {
		        WindowsKeyCode = keyCode,
		        EventType = CefKeyEventType.KeyDown
	        });
	        return Task.CompletedTask;
        }

        public Task SendKeyChars(string chars)
        {
	        foreach (char c in chars)
	        {
		        cefClient.KeyEvent(new CefKeyEvent
		        {
#if WINDOWS
					WindowsKeyCode = c,
#else
			        Character = c,
#endif
			        EventType = CefKeyEventType.Char
		        });
	        }
	        return Task.CompletedTask;
        }

        public Task SendMouseMoveEvent(int x, int y)
        {
	        Logger.Debug("Got mouse move event.");
	        cefClient.MouseMoveEvent(new CefMouseEvent
	        {
		        X = x,
		        Y = y
	        });
	        return Task.CompletedTask;
        }

        public Task SendSendScrollEvent(int x, int y, int scroll)
        {
	        cefClient.MouseScrollEvent(new CefMouseEvent
	        {
		        X = x,
		        Y = y
	        }, scroll);
	        return Task.CompletedTask;
        }

        public Task SendMouseClickDownEvent(int x, int y, int clickCount, MouseClickType clickType)
        {
	        cefClient.MouseClickEvent(new CefMouseEvent
	        {
		        X = x,
		        Y = y
	        }, clickCount, (CefMouseButtonType)clickType, false);
	        return Task.CompletedTask;
        }

        public Task SendMouseClickUpEvent(int x, int y, int clickCount, MouseClickType clickType)
        {
	        cefClient.MouseClickEvent(new CefMouseEvent
	        {
		        X = x,
		        Y = y
	        }, clickCount, (CefMouseButtonType)clickType, true);
	        return Task.CompletedTask;
        }

        public Task LoadUrl(string url)
        {
	        cefClient.LoadUrl(url);
	        return Task.CompletedTask;
        }

        public Task LoadHtml(string html)
        {
	        cefClient.LoadHtml(html);
	        return Task.CompletedTask;
        }

        public Task ExecuteJs(string js)
        {
	        cefClient.ExecuteJs(js);
	        return Task.CompletedTask;
        }
        
        #endregion
    }
}