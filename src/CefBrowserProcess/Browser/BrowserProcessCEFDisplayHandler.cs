using System;
using CefBrowserProcess.Core;
using Xilium.CefGlue;

namespace CefBrowserProcess.Browser
{
    public class BrowserProcessCEFDisplayHandler : CefDisplayHandler
    {
        protected override bool OnConsoleMessage(CefBrowser browser, CefLogSeverity level, string message, string source, int line)
        {
            switch (level)
            {
                case CefLogSeverity.Disable:
                    break;
                case CefLogSeverity.Default:
                case CefLogSeverity.Info:
                    Logger.Info($"CEF: {message}");
                    break;
                case CefLogSeverity.Warning:
                    Logger.Warn($"CEF: {message}");
                    break;
                case CefLogSeverity.Error:
                case CefLogSeverity.Fatal:
                    Logger.Error($"CEF: {message}");
                    break;
                case CefLogSeverity.Verbose:
                    Logger.Debug($"CEF: {message}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }

            return true;
        }
    }
}