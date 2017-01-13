namespace CompressionTest
{
    public class HttpListenerTest : TestBase
    {
        public HttpListenerTest()
            : base((appFunc, port) => new HttpListenerHost(appFunc, port))
        {
        }
    }
}