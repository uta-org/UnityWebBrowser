using System;
using System.Net;
using CefBrowserProcess.Models;
using CoreRPC;
using CoreRPC.Routing;
using CoreRPC.Transport;
using CoreRPC.Transport.Tcp;
using UnityWebBrowser.Shared;

namespace CefBrowserProcess.Core
{
    public class CommunicationsManager : IDisposable
    {
        private readonly TcpHost host;
        private readonly IPEndPoint ipEndPoint;
        
        public CommunicationsManager(LaunchArguments arguments, CefManager browser)
        {
            DefaultTargetSelector router = new DefaultTargetSelector();
            router.Register<IBrowser, CefManager>(browser);

            IRequestHandler engine = new Engine().CreateRequestHandler(router);
            ipEndPoint = IPEndPoint.Parse($"127.0.0.1:{arguments.Port}");
            host = new TcpHost(engine);
        }

        public void StartListening()
        {
            host.StartListening(ipEndPoint);
        }

        public void Dispose()
        {
            host.StopListening();
            host.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}