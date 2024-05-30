using SmsActivate.API;
using SmsActivate.API.Model;
using System.Collections.Generic;
using System.IO;

namespace SMSActivate.API.Tests
{
    public class RemoteSupportApiTest
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
        public void TestGetCountries()
        {
            var task = ApiClient.GetCountries();
            task.Wait();
            var result = task.Result;
            var payload = result.Data;
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload.Count, Is.Not.EqualTo(0));
            GenerateCountryCodes(payload);
        }        

        [Test]
        public void TestGetCountriesShort()
        {
            var task = ApiClient.GetCountriesShort();
            task.Wait();
            var result = task.Result;
            var payload = result.Data;
            if (result.Error != null && result.Error.StockApiErrResponse.StartsWith("<!DOCTYPE html>"))
            {
                //Anti-fraud
                return;
            }
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload.Count, Is.Not.EqualTo(0));
        }

        [Test]
        public void TestGetServices()
        {
            var task = ApiClient.GetServices();
            task.Wait();
            var result = task.Result;
            var payload = result.Data;
            if (result.Error != null && result.Error.StockApiErrResponse.StartsWith("<!DOCTYPE html>"))
            {
                //Anti-fraud
                return;
            }
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload.Count, Is.Not.EqualTo(0));
        }

        [Test]
        public void TestGetOperators()
        {
            var task = ApiClient.GetOperators();
            task.Wait();
            var result = task.Result;
            var payload = result.Data;
            if (result.Error != null && result.Error.StockApiErrResponse.StartsWith("<!DOCTYPE html>"))
            {
                //Anti-fraud
                return;
            }
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload.Count, Is.Not.EqualTo(0));
        }

        [Test]
        public void TestGetActivationServiceOffersV2()
        {
            var task = ApiClient.GetActivationServiceOffersV2(serviceId: TestConstants.ServiceId, forward: false);
            task.Wait();
            var result = task.Result;
            var payload = result.Data;
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload.Count, Is.Not.EqualTo(0));
        }

        [Test]
        public void TestGetActivationServiceOffersV3()
        {
            var task = ApiClient.GetActivationServiceOffersV3(serviceId: TestConstants.ServiceId, forward: false, freePrice: false);
            task.Wait();
            var result = task.Result;
            var payload = result.Data;
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload.Count, Is.Not.EqualTo(0));
        }

        [Test]
        public void TestGetRentServicesForCountry()
        {
            var task = ApiClient.GetRentServiceOffers(rentDuration: SARentDuration.Day, countryId: TestConstants.CountryId);
            task.Wait();
            var result = task.Result;
            var payload = result.Data;
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload.Count, Is.Not.EqualTo(0));
        }

        [Test]
        public void TestGetRentServiceForCountryShort()
        {
            var task = ApiClient.GetRentServiceShortOffers(rentDuration: SARentDuration.Day, countryId: TestConstants.CountryId);
            task.Wait();
            var result = task.Result;
            var payload = result.Data;
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload.Count, Is.Not.EqualTo(0));
        }

        private static void GenerateCountryCodes(Dictionary<ushort, SACountry> countries)
        {
            var filename = "codes.csv";
            var streamWriter = new StreamWriter(filename);
            streamWriter.Write("key|code|note");
            foreach (var entry in countries)
            {
                streamWriter.Write('\n' + entry.Value.NameEn.ToLower() + "|-1|");
                streamWriter.Write('\n' + entry.Value.NameRus.ToLower() + "|-1|");
                streamWriter.Write('\n' + entry.Key.ToString() + "|-1|" + entry.Value.NameEn + " ID (SA)");
            }
            streamWriter.Flush();
            streamWriter.Close();
            streamWriter.Dispose();
        }
    }
}