namespace CompressionTest
{
    public class WebListenerTest : TestBase
    {
        public WebListenerTest()
            : base((appFunc, port) => new WebListenerHost(appFunc, port))
        {
            
        }
    }
}