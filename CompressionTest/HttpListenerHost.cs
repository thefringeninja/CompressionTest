using System;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Hosting.ServerFactory;
using Owin;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>,
    System.Threading.Tasks.Task>;

namespace CompressionTest
{
    internal class HttpListenerHost : IDisposable
    {
        private readonly IDisposable _server;

        public string Url { get; }

        public HttpListenerHost(AppFunc appFunc, int port)
        {
            Url = $"http://localhost:{port}";

            _server = WebApp.Start(
                new StartOptions(Url)
                {
                    Settings =
                    {
                        [typeof(IServerFactoryLoader).FullName] = typeof(ServerFactoryLoader).AssemblyQualifiedName,
                        [typeof(IServerFactoryAdapter).FullName] = typeof(ServerFactoryAdapter).AssemblyQualifiedName
                    }
                }, builder => { builder.Run(context => appFunc(context.Environment)); });
        }

        public void Dispose() => _server.Dispose();
    }
}