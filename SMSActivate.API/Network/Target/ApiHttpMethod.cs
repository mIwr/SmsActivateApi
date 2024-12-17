using System.Net.Http;

namespace SmsActivate.API.Network
{
    internal static class ApiHttpMethod
    {
        private const string _getMethod = "GET";
        //private const string _postMethod = "POST";

        internal static string MethodString(this ApiTarget target)
        {
            switch(target)
            {
                case ApiTarget.Countries: return _getMethod;
                case ApiTarget.CountriesShort: return _getMethod;
                case ApiTarget.OperatorsWeb: return _getMethod;
                case ApiTarget.OperatorsApi: return _getMethod;
                case ApiTarget.Services: return _getMethod;
                case ApiTarget.RentServiceOffers: return _getMethod;
                case ApiTarget.ActivationServiceCountryOffersV3: return _getMethod;
                case ApiTarget.ActivationServiceCountryOffersV2: return _getMethod;
                case ApiTarget.RentServiceShortOffers: return _getMethod;
                case ApiTarget.ProfileBalance: return _getMethod;
                case ApiTarget.ProfileBalanceAndCashBack: return _getMethod;
                case ApiTarget.ProfileBalanceAndCashBackV2: return _getMethod;
                case ApiTarget.ProfileActiveActivations: return _getMethod;
                case ApiTarget.ProfileActiveActivationsPaged: return _getMethod;
                case ApiTarget.NewActivation: return _getMethod;
                case ApiTarget.NewActivationV2: return _getMethod;
                case ApiTarget.ActivationStatus: return _getMethod;
                case ApiTarget.ActivationStatusChange: return _getMethod;
            }
            return _getMethod;
        }

        internal static HttpMethod Method(this ApiTarget target)
        {
            switch (target)
            {
                case ApiTarget.Countries: return HttpMethod.Get;
                case ApiTarget.CountriesShort: return HttpMethod.Get;
                case ApiTarget.OperatorsWeb: return HttpMethod.Get;
                case ApiTarget.OperatorsApi: return HttpMethod.Get;
                case ApiTarget.Services: return HttpMethod.Get;
                case ApiTarget.RentServiceOffers: return HttpMethod.Get;
                case ApiTarget.ActivationServiceCountryOffersV3: return HttpMethod.Get;
                case ApiTarget.ActivationServiceCountryOffersV2: return HttpMethod.Get;
                case ApiTarget.RentServiceShortOffers: return HttpMethod.Get;
                case ApiTarget.ProfileBalance: return HttpMethod.Get;
                case ApiTarget.ProfileBalanceAndCashBack: return HttpMethod.Get;
                case ApiTarget.ProfileBalanceAndCashBackV2: return HttpMethod.Get;
                case ApiTarget.ProfileActiveActivations: return HttpMethod.Get;
                case ApiTarget.ProfileActiveActivationsPaged: return HttpMethod.Get;
                case ApiTarget.NewActivation: return HttpMethod.Get;
                case ApiTarget.NewActivationV2: return HttpMethod.Get;
                case ApiTarget.ActivationStatus: return HttpMethod.Get;
                case ApiTarget.ActivationStatusChange: return HttpMethod.Get;
            }
            return HttpMethod.Get;
        }
    }
}
