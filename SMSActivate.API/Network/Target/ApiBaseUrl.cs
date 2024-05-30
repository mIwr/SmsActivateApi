namespace SmsActivate.API.Network
{
    internal static class ApiTargetBaseUrl
    {
        internal const string ApiBaseUrl = "https://sms-activate.org/";
        internal const string SpecApiBaseUrl = "https://api.sms-activate.org/";

        internal static string BaseUrl (this ApiTarget target)
        {
            switch (target)
            {
                case ApiTarget.Countries: return ApiBaseUrl;
                case ApiTarget.CountriesShort: return ApiBaseUrl;
                case ApiTarget.OperatorsWeb: return ApiBaseUrl;
                case ApiTarget.OperatorsApi: return SpecApiBaseUrl;
                case ApiTarget.Services: return ApiBaseUrl;
                case ApiTarget.RentServiceOffers: return ApiBaseUrl;
                case ApiTarget.ActivationServiceCountryOffersV3: return ApiBaseUrl;
                case ApiTarget.ActivationServiceCountryOffersV2: return ApiBaseUrl;
                case ApiTarget.RentServiceShortOffers: return ApiBaseUrl;

                case ApiTarget.ProfileBalance: return SpecApiBaseUrl;
                case ApiTarget.ProfileBalanceAndCashBack: return SpecApiBaseUrl;
                case ApiTarget.ProfileBalanceAndCashBackV2: return ApiBaseUrl;

                case ApiTarget.ProfileActiveActivations: return SpecApiBaseUrl;
                case ApiTarget.ProfileActiveActivationsPaged: return ApiBaseUrl;
                case ApiTarget.NewActivation: return ApiBaseUrl;
                case ApiTarget.NewActivationV2: return ApiBaseUrl;
                case ApiTarget.ActivationStatus: return SpecApiBaseUrl;
                case ApiTarget.ActivationStatusChange: return SpecApiBaseUrl;
            }
            return ApiBaseUrl;
        }
    }
}
