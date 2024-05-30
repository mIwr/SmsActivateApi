using SmsActivate.API.Model;
using SmsActivate.API.Network;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SmsActivate.API
{
    public partial class SAClient
    {
        /// <summary>
        /// Get countries list with short info (Anonymous)
        /// </summary>
        /// <param name="language">Country name language</param>
        /// <returns>API response result with the contries list</returns>
        public async Task<Result<Dictionary<ushort, SACountryShort>, ApiError>> GetCountriesShort(string language = "en")
        {
            var res = await ApiSupport.GetCountriesShort(language);
            var payload = res.Data;
            if (payload != null && payload.Count > 0 && CachedActivationServicesInfo.Count == 0)
            {
                foreach (var entry in payload)
                {
                    if (!CachedCountriesInfo.ContainsKey(entry.Key))
                    {
                        CachedCountriesInfo.Add(entry.Key, new SACountry(entry.Key, entry.Value.Name, entry.Value.Name, entry.Value.Name, visible: true, retry: false, rent: false, multiService: false));
                    }                    
                }
            }
            return res;
        }

        /// <summary>
        /// Get countries list (Anonymous)
        /// </summary>
        /// <returns>API response result with the contries list</returns>
        public async Task<Result<Dictionary<ushort, SACountry>, ApiError>> GetCountries()
        {
            var res = await ApiSupport.GetCountries();
            var payload = res.Data;
            if (payload != null && payload.Count > 0)
            {
                CachedCountriesInfo = new Dictionary<ushort, SACountry>(payload);
            }
            return res;
        }

        /// <summary>
        /// Get services list (Anonymous)
        /// </summary>
        /// <returns>API response result with the services list</returns>
        public async Task<Result<Dictionary<string, SAActivationService>, ApiError>> GetServices()
        {
            var res = await ApiSupport.GetServices();
            var payload = res.Data;
            if (payload != null && payload.Count > 0)
            {
                CachedActivationServicesInfo = new Dictionary<string, SAActivationService>(payload);                
            }
            return res;
        }

        /// <summary>
        /// Get mobile operators list
        /// </summary>
        /// <returns>API response result with the operators list</returns>
        public async Task<Result<Dictionary<string, SAOperator>, ApiError>> GetOperators()
        {
            var res = await ApiSupport.GetOperators();
            var payload = res.Data;
            if (payload == null)
            {
#if DEBUG
                Trace.WriteLine("Anonymous getOperators fail");
                var err = res.Error;
                if (err != null)
                {
                    Trace.WriteLine(err);
                }
                Trace.WriteLine("Request operators info in stock mode");
#endif
                res = await ApiSupport.GetOperators(Token);
                payload = res.Data;
                if (payload != null && payload.Count > 0)
                {
                    CachedOperatorsInfo = new Dictionary<string, SAOperator>(payload);
                }
            }
            else if (payload.Count > 0)
            {
                CachedOperatorsInfo = new Dictionary<string, SAOperator>(payload);
            }
            return res;
        }

        /// <summary>
        /// Get countries' activation offers info for defined service (Anonymous)
        /// </summary>
        /// <param name="serviceId">Service short name (ID)</param>
        /// <param name="forward">Activation can be forwarded flag</param>
        /// <returns>API response result with the countries' activation offers</returns>
        public async Task<Result<Dictionary<ushort, SAActivationServiceOfferV2>, ApiError>> GetActivationServiceOffersV2(string serviceId, bool forward)
        {
            if (CachedActivationServicesInfo.Count > 0 && !CachedActivationServicesInfo.ContainsKey(serviceId))
            {
                return new Result<Dictionary<ushort, SAActivationServiceOfferV2>, ApiError>(error: ApiErrorEnum.BadService.AsException(apiResponse: string.Empty));
            }
            var iforward = forward ? 1 : 0;
            return await ApiSupport.GetCountriesForActivationServiceV2(serviceId, iforward);
        }

        /// <summary>
        /// Get countries' activation offers info for defined service (Anonymous)
        /// </summary>
        /// <param name="serviceId">>Service short name (ID)</param>
        /// <param name="forward">Activation can be forwarded flag</param>
        /// <param name="freePrice">Free Price feature flag</param>
        /// <returns>API response result with the countries' activation offers</returns>
        public async Task<Result<Dictionary<ushort, SAActivationServiceOfferV3>, ApiError>> GetActivationServiceOffersV3(string serviceId, bool forward, bool freePrice)
        {
            if (CachedActivationServicesInfo.Count > 0 && !CachedActivationServicesInfo.ContainsKey(serviceId))
            {
                return new Result<Dictionary<ushort, SAActivationServiceOfferV3>, ApiError>(error: ApiErrorEnum.BadService.AsException(apiResponse: string.Empty));
            }
            var iforward = forward ? 1 : 0;
            return await ApiSupport.GetCountriesForAcrivationServiceV3(serviceId, iforward, freePrice);
        }

        /// <summary>
        /// Get rent service offers (Anonymous)
        /// </summary>
        /// <param name="countryId">Country filter. Optional</param>
        /// <param name="rentDuration">Rent duration variant</param>
        /// <returns>API response result with the services' rent offers</returns>
        public async Task<Result<Dictionary<string, SARentServiceOffer>, ApiError>> GetRentServiceOffers(SARentDuration rentDuration, ushort? countryId = null)
        {
            if (countryId != null && CachedCountriesInfo.Count > 0 && !CachedCountriesInfo.ContainsKey(countryId.Value))
            {
                return new Result<Dictionary<string, SARentServiceOffer>, ApiError>(error: ApiErrorEnum.BadCountry.AsException(apiResponse: string.Empty));
            }
            var rentHours = rentDuration.TotalHours();
            var res = await ApiSupport.GetRentServiceOffers(rentHours, countryId);
            var payload = res.Data;
            if (payload != null && payload.Count > 0)
            {
                foreach (var entry in payload)
                {
                    if (CachedRentServicesInfo.ContainsKey(entry.Key))
                    {
                        CachedRentServicesInfo[entry.Key] = new SAService(entry.Key, entry.Value.Name);
                        continue;
                    }
                    CachedRentServicesInfo.Add(entry.Key, new SAService(entry.Key, entry.Value.Name));
                }
            }
            return res;
        }

        /// <summary>
        /// Get rent service for defined country (Anonymous)
        /// </summary>
        /// <param name="countryId">Country filter. Optional</param>
        /// <param name="serviceId">Service filter. Optional</param>
        /// <param name="rentDuration">Rent duration variant</param>
        /// <returns>API response result with the services' rent offers</returns>
        public async Task<Result<Dictionary<string, SARentServiceOffer>, ApiError>> GetRentServiceShortOffers(SARentDuration rentDuration, ushort? countryId = null, string? serviceId = null)
        {
            if (countryId != null && CachedCountriesInfo.Count > 0 && !CachedCountriesInfo.ContainsKey(countryId.Value))
            {
                return new Result<Dictionary<string, SARentServiceOffer>, ApiError>(error: ApiErrorEnum.BadCountry.AsException(apiResponse: string.Empty));
            }
            var rentHours = rentDuration.TotalHours();
            var res = await ApiSupport.GetRentServiceShortOffers(rentHours, countryId, serviceId);
            var payload = res.Data;
            if (payload != null && payload.Count > 0)
            {
                foreach (var entry in payload)
                {
                    if (CachedRentServicesInfo.ContainsKey(entry.Key))
                    {
                        continue;
                    }
                    CachedRentServicesInfo.Add(entry.Key, new SAService(entry.Key, entry.Value.Name));
                }
            }
            return res;
        }
    }
}
