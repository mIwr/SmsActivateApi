using SmsActivate.API;

namespace SMSActivate.API.Tests
{
    internal class RemoteProfileApiTest
    {
        SAClient ApiClient;

        [OneTimeSetUp]
        public void Setup()
        {
            ApiClient = new SAClient(token: TestCredentials.ApiToken);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            ApiClient.Dispose();
        }

        [Test]
        public void TestGetProfileBalance()
        {
            var task = ApiClient.GetProfileBalance();
            task.Wait();
            var result = task.Result;
            var payload = result.Data;
            Assert.That(result.Error, Is.Null);
            Assert.That(payload, Is.GreaterThanOrEqualTo(0.0));
        }

        [Test]
        public void TestGetProfileBalanceAndCashBackV1()
        {
            var task = ApiClient.GetLegacyProfileBalanceAndCashback();
            task.Wait();
            var result = task.Result;
            var payload = result.Data;
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload.Total, Is.GreaterThanOrEqualTo(0.0));
        }

        [Test]
        public void TestGetProfileBalanceAndCashBack()
        {
            var task = ApiClient.GetProfileBalanceAndCashback();
            task.Wait();
            var result = task.Result;
            var payload = result.Data;
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload.Total, Is.GreaterThanOrEqualTo(0.0));
        }
    }
}
