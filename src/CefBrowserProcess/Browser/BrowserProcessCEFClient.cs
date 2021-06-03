using System;
using CefBrowserProcess.Models;
using Xilium.CefGlue;

namespace CefBrowserProcess.Browser
{
	/// <summary>
	///		Offscreen CEF
	/// </summary>
	public class BrowserProcessCEFClient : CefClient, IDisposable
	{
		private CefBrowser browser;
		private CefBrowserHost browserHost;
		private CefFrame mainFrame;
		
		private readonly BrowserProcessCEFLoadHandler loadHandler;
		private readonly BrowserProcessCEFRenderHandler renderHandler;
		private readonly BrowserProcessCEFLifespanHandler lifespanHandler;
		private readonly BrowserProcessCEFDisplayHandler displayHandler;
		private readonly BrowserProcessCEFRequestHandler requestHandler;

		///  <summary>
		/// 		Creates a new <see cref="BrowserProcessCEFClient"/> instance
		///  </summary>
		///  <param name="size">The size of the window</param>
		///  <param name="proxySettings"></param>
		public BrowserProcessCEFClient(CefSize size, ProxySettings proxySettings)
		{
			//Setup our handlers
			loadHandler = new BrowserProcessCEFLoadHandler(this);
			renderHandler = new BrowserProcessCEFRenderHandler(size);
			lifespanHandler = new BrowserProcessCEFLifespanHandler();
			lifespanHandler.AfterCreated += cefBrowser =>
			{
				browser = cefBrowser;
				browserHost = cefBrowser.GetHost();
				mainFrame = cefBrowser.GetMainFrame();
			};
			displayHandler = new BrowserProcessCEFDisplayHandler();
			requestHandler = new BrowserProcessCEFRequestHandler(proxySettings);
		}

		/// <summary>
		///		Destroys the <see cref="BrowserProcessCEFClient"/> instance
		/// </summary>
		public void Dispose()
		{
			browserHost?.CloseBrowser(true);
			browserHost?.Dispose();
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///		Gets the pixel data of the CEF window
		/// </summary>
		/// <returns></returns>
		public byte[] GetPixels()
		{
			if(browserHost == null)
				return Array.Empty<byte>();

			return renderHandler.Pixels;
		}

		public void KeyEvent(CefKeyEvent keyEvent)
		{
			browserHost.SendKeyEvent(keyEvent);
		}

		public void MouseMoveEvent(CefMouseEvent mouseEvent)
		{
			browserHost.SendMouseMoveEvent(mouseEvent, false);
		}

		public void MouseClickEvent(CefMouseEvent mouseEvent, int clickCount, CefMouseButtonType button, bool mouseUp)
		{
			browserHost.SendMouseClickEvent(mouseEvent, button, mouseUp, clickCount);
		}

		public void MouseScrollEvent(CefMouseEvent mouseEvent, int scroll)
		{
			browserHost.SendMouseWheelEvent(mouseEvent, 0, scroll);
		}

		public void LoadUrl(string url)
		{
			mainFrame.LoadUrl(url);
		}

		public void LoadHtml(string html)
		{
			mainFrame.LoadUrl($"data:text/html,{html}");
		}

		public void ExecuteJs(string js)
		{
			mainFrame.ExecuteJavaScript(js, "", 0);
		}

		private void GoBack()
		{
			if(browser.CanGoBack)
				browser.GoBack();
		}

		private void GoForward()
		{
			if(browser.CanGoForward)
				browser.GoForward();
		}

		private void Refresh()
		{
			browser.Reload();
		}

		protected override CefLoadHandler GetLoadHandler()
		{
			return loadHandler;
		}

		protected override CefRenderHandler GetRenderHandler()
		{
			return renderHandler;
		}

		protected override CefLifeSpanHandler GetLifeSpanHandler()
		{
			return lifespanHandler;
		}

		protected override CefDisplayHandler GetDisplayHandler()
		{
			return displayHandler;
		}

		protected override CefRequestHandler GetRequestHandler()
		{
			return requestHandler;
		}
	}
}