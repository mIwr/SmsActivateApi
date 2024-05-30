using SmsActivate.API;
using SmsActivate.API.Network;

namespace SMSActivate.API.Tests
{
    internal class RemoteNotifierTest
    {
        private class WebListener : IWebClientListener
        {
            public int CID { get; private set; }

            public void Handle(int cid, string request, int statusCode, string response)
            {
                CID = cid;
            }
        }

        private class ExceptionListener : IExceptionListener
        {
            public ApiErrorEnum LastErr { get; private set; }
            public void Handle(string errorFromSrv, ApiError parsed)
            {
                LastErr = parsed.ErrType;
            }
        }

        SAClient ApiClient;
        WebListener WebL;
        ExceptionListener ExceptionL;

        [OneTimeSetUp]
        public void Setup()
        {
            ApiClient = new SAClient(token: "12345");
            WebL = new();
            ExceptionL = new();
            ApiClient.AddWebClientListener(WebL);
            ApiClient.AddExceptionListener(ExceptionL);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            ApiClient.Dispose();
        }

        [Test]
        public void TestWebListener()
        {
            var currCid = WebL.CID;
            var task = ApiClient.GetCountries();
            task.Wait();
            Assert.That(currCid, Is.Not.EqualTo(WebL.CID));
        }

        [Test]
        public void TestExceptionListener()
        {
            var currErr = ExceptionL.LastErr;
            var task = ApiClient.GetProfileActiveActivations();
            task.Wait();
            Assert.That(currErr, Is.Not.EqualTo(ExceptionL.LastErr));
        }
    }
}
