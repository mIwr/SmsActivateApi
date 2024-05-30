using SmsActivate.API;

namespace SMSActivate.API.Tests
{
    internal class RemoteActivationApiTest
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
        public void TestRequestNewActivationV1()
        {
            var balanceTask = ApiClient.GetProfileBalance();
            balanceTask.Wait();
            var balance = balanceTask.Result?.Data;
            Assert.That(balance, Is.Not.Null);
            if (balance <= 0.0)
            {
                return;
            }
            var task = ApiClient.RequestLegacyNewActivation(TestConstants.ServiceId, TestConstants.CountryId);
            task.Wait();
            var result = task.Result;
            var payload = result.Data;
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload.ID, Is.GreaterThan(0));
        }

        [Test]
        public void TestRequestNewActivation()
        {
            var balanceTask = ApiClient.GetProfileBalance();
            balanceTask.Wait();
            var balance = balanceTask.Result?.Data;
            Assert.That(balance, Is.Not.Null);
            if (balance <= 0.0)
            {
                return;
            }
            var task = ApiClient.RequestNewActivation(TestConstants.ServiceId, TestConstants.CountryId); 
            task.Wait();
            var result = task.Result;
            var payload = result.Data;
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload.ID, Is.GreaterThan(0));
        }

        [Test]
        public void TestGetProfileActiveActivations()
        {
            var task = ApiClient.GetProfileActiveActivations();
            task.Wait();
            var result = task.Result;
            var payload = result.Data;
            Assert.That(payload, Is.Not.Null);
        }

        [Test]
        public void TestGetProfileActiveActivationsPaged()
        {
            var task = ApiClient.GetProfileActiveActivationsPaged();
            task.Wait();
            var result = task.Result;
            var payload = result.Data;
            Assert.That(payload, Is.Not.Null);
        }

        [Test]
        public void TestGetActivationStatus()
        {
            var task = ApiClient.GetProfileActiveActivations();
            task.Wait();
            var result = task.Result;
            var payload = result.Data;
            if (payload == null || payload.Length == 0)
            {
                return;
            }
            var activation = payload[0];
            var statusTask = ApiClient.GetActivationStatusWithSMS(activation.ID);
            statusTask.Wait();
            var statusRes = statusTask.Result;
            Assert.That(statusRes.Error, Is.Null);
            Assert.That(statusRes.Success, Is.True);
        }

        [Test]
        public void TestSetActivationStatus()
        {
            var task = ApiClient.GetProfileActiveActivations();
            task.Wait();
            var result = task.Result;
            var payload = result.Data;
            if (payload == null || payload.Length == 0)
            {
                return;
            }
            var activation = payload[0];
            var statusTask = ApiClient.ActivationCancel(activation.ID);
            statusTask.Wait();
            var statusRes = statusTask.Result;
            Assert.That(statusRes.Error, Is.Null);
            Assert.That(statusRes.Success, Is.True);
        }
    }
}
