using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Xunit;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>,
    System.Threading.Tasks.Task>;
namespace CompressionTest
{
    public abstract class TestBase : IDisposable
    {
        private readonly IDisposable _sut;
        private readonly int _port;

        protected string Url => $"http://localhost:{_port}";

        private static int s_NextPort = 8080;
        public static readonly string ResponseBody = new string('a', 10000);

        protected TestBase(Func<AppFunc, int, IDisposable> createSut)
        {
            _port = Interlocked.Increment(ref s_NextPort);

            _sut = createSut(async env =>
            {
                var context = new OwinContext(env);

                context.Response.Headers["Content-Encoding"] = "gzip";

                using (var stream = new MemoryStream())
                using (var gzipStream = new GZipStream(stream, CompressionMode.Compress))
                {
                    var buffer = Encoding.UTF8.GetBytes(ResponseBody);

                    await gzipStream.WriteAsync(buffer, 0, buffer.Length);

                    stream.Position = 0;

                    context.Response.ContentLength = stream.Length;

                    await stream.CopyToAsync(context.Response.Body, 8192, context.Request.CallCancelled);
                }
            }, _port);
        }

        [Fact]
        public async Task when_no_user_agent()
        {
            using (var client = new HttpClient()
            {
                BaseAddress = new Uri(Url),
            })
            {
                client.DefaultRequestHeaders.UserAgent.Clear();

                using (var response = await client.GetAsync("/"))
                {
                    Assert.Contains("gzip", response.Content.Headers.ContentEncoding);
                }
            }
        }

        [Fact]
        public async Task when_user_agent_has_gecko()
        {
            using (var client = new HttpClient()
            {
                BaseAddress = new Uri(Url),
            })
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36");

                using (var response = await client.GetAsync("/"))
                {
                    Assert.Contains("gzip", response.Content.Headers.ContentEncoding);
                }
            }
        }

        public void Dispose()
        {
            _sut.Dispose();
        }
    }
}