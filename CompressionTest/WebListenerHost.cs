using System;
using OwinWebListenerHost;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>,
    System.Threading.Tasks.Task>;

namespace CompressionTest
{
    internal class WebListenerHost : IDisposable
    {
        private readonly OwinWebListener _listener;

        public string Url { get; }

        public WebListenerHost(AppFunc appFunc, int port)
        {
            Url = $"http://localhost:{port}";

            _listener = new OwinWebListener();
            _listener.Start(appFunc, Url);
        }

        public void Dispose() => _listener.Dispose();
    }
}