namespace SmsActivate.API.Network
{
    internal enum ApiTarget: byte
    {
        //Anonymous
        Countries, 
        CountriesShort,
        OperatorsWeb,        
        Services,        
        RentServiceOffers, 
        RentServiceShortOffers, 
        ActivationServiceCountryOffersV2, 
        ActivationServiceCountryOffersV3,
        //Token access
        OperatorsApi,
        ProfileBalance,
        ProfileBalanceAndCashBack, 
        ProfileBalanceAndCashBackV2,
        ProfileActiveActivations, 
        ProfileActiveActivationsPaged, 
        NewActivation, 
        NewActivationV2, 
        ActivationStatus, 
        ActivationStatusChange
    }
}
