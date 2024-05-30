namespace SmsActivate.API.Network
{
    internal static class ApiReqPath
    {
        private const string _apiApiPrefix = "api/api.php";
        private const string _apiMobilePrefix = "stubs/apiMobile.php";
        private const string _apiRentPrefix = "stubs/rent.php";
        private const string _apiSpecPrefix = "stubs/handler_api.php";

        internal static string ReqPath(this ApiTarget target)
        {
            switch(target)
            {
                case ApiTarget.Countries: return _apiMobilePrefix + "?action=getCountries";
                case ApiTarget.CountriesShort: return _apiApiPrefix + "?act=getCountries";
                case ApiTarget.OperatorsWeb: return _apiApiPrefix + "?act=getOperatorsList";
                case ApiTarget.OperatorsApi: return _apiSpecPrefix + "?action=getOperators";
                case ApiTarget.Services: return _apiApiPrefix + "?act=getServicesList";
                case ApiTarget.RentServiceOffers: return _apiRentPrefix + "?action=getRentServices";
                case ApiTarget.ActivationServiceCountryOffersV3: return _apiMobilePrefix + "?action=getTopCountriesByServiceV3";
                case ApiTarget.ActivationServiceCountryOffersV2: return _apiMobilePrefix + "?action=countriesStackRender&v=2";
                case ApiTarget.RentServiceShortOffers: return _apiRentPrefix + "?action=updateRentCount";

                case ApiTarget.ProfileBalance: return _apiSpecPrefix + "?action=getBalance";
                case ApiTarget.ProfileBalanceAndCashBack: return _apiSpecPrefix + "?action=getBalanceAndCashBack";
                case ApiTarget.ProfileBalanceAndCashBackV2: return _apiSpecPrefix + "?action=getBalanceAndCashBackV2";

                case ApiTarget.ProfileActiveActivations: return _apiSpecPrefix + "?action=getActiveActivations";
                case ApiTarget.ProfileActiveActivationsPaged: return _apiSpecPrefix + "?&action=getListOfActiveActivations";
                case ApiTarget.NewActivation: return _apiSpecPrefix + "?action=getNumber";
                case ApiTarget.NewActivationV2: return _apiSpecPrefix + "?action=getNumberV2";
                case ApiTarget.ActivationStatus: return _apiSpecPrefix + "?action=getStatus";
                case ApiTarget.ActivationStatusChange: return _apiSpecPrefix + "?action=setStatus";
            }
            return string.Empty;
        }

        /*

GET /stubs/handler_api.php?action=getListOfActiveRent&start=0&length=50&refresh_token=[edited]&sessionId=[edited]&userid=[edited]&owner=5 HTTP/2
Host: sms-activate.org
User-Agent: Dart/3.0 (dart:io)
Accept-Encoding: gzip, deflate, br
Adduseroptions: true
         
GET /stubs/apiMobile.php?action=getAllServices HTTP/2
Host: sms-activate.org
User-Agent: Dart/3.0 (dart:io)
Accept-Encoding: gzip, deflate

GET /stubs/apiMobile.php?action=getAllServicesAndAllCountries HTTP/1.1
user-agent: Dart/3.0 (dart:io)
Accept-Encoding: gzip, deflate
host: sms-activate.org
Connection: close
         */
    }
}
