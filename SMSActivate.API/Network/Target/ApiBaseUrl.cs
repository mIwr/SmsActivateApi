namespace SmsActivate.API.Network
{
    internal static class ApiTargetBaseUrl
    {
        internal static bool UseApiBaseUrl (this ApiTarget target)
        {
            switch (target)
            {
                case ApiTarget.Countries: return false;
                case ApiTarget.CountriesShort: return false;
                case ApiTarget.OperatorsWeb: return false;//https://sms-activate.org/
                case ApiTarget.OperatorsApi: return true;//https://api.sms-activate.org/
                case ApiTarget.Services: return false;
                case ApiTarget.RentServiceOffers: return false;
                case ApiTarget.ActivationServiceCountryOffersV3: return false;
                case ApiTarget.ActivationServiceCountryOffersV2: return false;
                case ApiTarget.RentServiceShortOffers: return false;

                case ApiTarget.ProfileBalance: return true;
                case ApiTarget.ProfileBalanceAndCashBack: return true;
                case ApiTarget.ProfileBalanceAndCashBackV2: return false;

                case ApiTarget.ProfileActiveActivations: return true;
                case ApiTarget.ProfileActiveActivationsPaged: return false;
                case ApiTarget.NewActivation: return false;
                case ApiTarget.NewActivationV2: return false;
                case ApiTarget.ActivationStatus: return true;
                case ApiTarget.ActivationStatusChange: return true;
            }
            return false;
        }
    }
}
