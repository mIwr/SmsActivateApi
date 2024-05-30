using SmsActivate.API.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmsActivate.API.Network
{
    internal static class ApiSupport
    {
        internal static async Task<Result<Dictionary<ushort, SACountry>, ApiError>> GetCountries()
        {
            var target = ApiTarget.Countries;
            var request = Client.BuildRequest(target);            
            var response = await GlobalEnv.ApiClient.RequestAsGenResponse(request);
            var err = response.Error;
            if (err != null)
            {
                return new Result<Dictionary<ushort, SACountry>, ApiError>(err);
            }
            var dat = response.Data;
            if (string.IsNullOrEmpty(dat))
            {
                return new Result<Dictionary<ushort, SACountry>, ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            }
            var dict = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, dynamic>>>(dat) ?? new Dictionary<string, Dictionary<string, dynamic>>();
            var res = new Dictionary<ushort, SACountry>();
            foreach (var entry in dict)
            {
                var parsed = SACountry.From(entry.Value);
                if (parsed == null)
                {
#if DEBUG
                    Trace.WriteLine("Invalid json for ID " + entry.Key);
#endif
                    continue;
                }
                res.Add(parsed.ID, parsed);
            }
            return new Result<Dictionary<ushort, SACountry>, ApiError>(res);
        }

        internal static async Task<Result<Dictionary<ushort, SACountryShort>, ApiError>> GetCountriesShort(string language)
        {
            var target = ApiTarget.CountriesShort;
            var request = Client.BuildRequest(target, additionalReqParameters: new Dictionary<string, dynamic>
            {
                ["lang"] = language
            });
            var response = await GlobalEnv.ApiClient.RequestAsGenResponse(request);
            var err = response.Error;
            if (err != null)
            {
                return new Result<Dictionary<ushort, SACountryShort>, ApiError>(err);
            }
            var dat = response.Data;
            if (string.IsNullOrEmpty(dat))
            {
                return new Result<Dictionary<ushort, SACountryShort>, ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            }
            var dict = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(dat) ?? new Dictionary<string, dynamic>();
            if (!dict.ContainsKey("data"))
            {
                return new Result<Dictionary<ushort, SACountryShort>, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Not found 'data' key"));
            }
            JsonElement obj = dict["data"];
            var countriesDict = obj.Deserialize<Dictionary<string, string>>() ?? new Dictionary<string, string>();
            var res = new Dictionary<ushort, SACountryShort>();
            foreach (var entry in countriesDict)
            {
                if (!ushort.TryParse(entry.Key, out var countryId))
                {
#if DEBUG
                    Trace.WriteLine("Country ID parse fail from " + entry.Key);
#endif
                    continue;
                }
                res.Add(countryId, new SACountryShort(countryId, entry.Value));
            }
            return new Result<Dictionary<ushort, SACountryShort>, ApiError>(res);
        }

        internal static async Task<Result<Dictionary<string, SAOperator>, ApiError>> GetOperators()
        {
            var target = ApiTarget.OperatorsWeb;
            var request = Client.BuildRequest(target);
            var response = await GlobalEnv.ApiClient.RequestAsGenResponse(request);
            var err = response.Error;
            if (err != null)
            {
                return new Result<Dictionary<string, SAOperator>, ApiError>(err);
            }
            var dat = response.Data;
            if (string.IsNullOrEmpty(dat))
            {
                return new Result<Dictionary<string, SAOperator>, ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            }
            var dict = new Dictionary<string, dynamic>();
            try
            {
                dict = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(dat) ?? new Dictionary<string, dynamic>();
            }
            catch (Exception ex)
            {
#if DEBUG
                Trace.WriteLine(ex);
#endif
                return new Result<Dictionary<string, SAOperator>, ApiError>(error: ApiErrorEnum.BadResponse.AsException(dat, message: "Unable parse as JSON dictionary"));
            }
            if (!dict.ContainsKey("data"))
            {
                return new Result<Dictionary<string, SAOperator>, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Not found 'data' key"));
            }
            JsonElement obj = dict["data"];
            var operatorsDict = obj.Deserialize<Dictionary<string, dynamic>>() ?? new Dictionary<string, dynamic>();
            var res = new Dictionary<string, SAOperator>();
            foreach (var entry in operatorsDict)
            {
                try
                {
                    obj = entry.Value;
                    var operatorDict = obj.Deserialize<Dictionary<string, dynamic>>() ?? new Dictionary<string, dynamic>();
                    var parsed = SAOperator.FromWebAPI(operatorDict, id: entry.Key);
                    if (parsed == null)
                    {
#if DEBUG
                        Trace.WriteLine("Invalid JSON");
                        Trace.WriteLine(entry);
#endif
                        continue;
                    }
                    res[entry.Key] = parsed;
                }
                catch (Exception ex)
                {
#if DEBUG
                    Trace.WriteLine(ex);
#endif
                }
            }
            return new Result<Dictionary<string, SAOperator>, ApiError>(res);
        }

        internal static async Task<Result<Dictionary<string, SAOperator>, ApiError>> GetOperators(string apiToken, string? countryId = null)
        {
            var additionaLReqParams = new Dictionary<string, string>
            {
                ["api_key"] = apiToken
            };
            if (!string.IsNullOrEmpty(countryId))
            {
                additionaLReqParams["country"] = countryId;
            }
            var target = ApiTarget.OperatorsWeb;
            var request = Client.BuildRequest(target);
            var response = await GlobalEnv.ApiClient.RequestAsGenResponse(request);
            var err = response.Error;
            if (err != null)
            {
                return new Result<Dictionary<string, SAOperator>, ApiError>(err);
            }
            var dat = response.Data;
            if (string.IsNullOrEmpty(dat))
            {
                return new Result<Dictionary<string, SAOperator>, ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            }
            var dict = new Dictionary<string, dynamic>();
            try
            {
                dict = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(dat) ?? new Dictionary<string, dynamic>();
            }
            catch (Exception ex)
            {
#if DEBUG
                Trace.WriteLine(ex);
#endif
                return new Result<Dictionary<string, SAOperator>, ApiError>(error: ApiErrorEnum.BadResponse.AsException(dat, message: "Unable parse as JSON dictionary"));
            }
            if (!dict.ContainsKey("countryOperators"))
            {
                return new Result<Dictionary<string, SAOperator>, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Not found 'countryOperators' key"));
            }
            JsonElement obj = dict["countryOperators"];
            var countriesDict = obj.Deserialize<Dictionary<string, dynamic>>() ?? new Dictionary<string, dynamic>();            
            var countryOperators = new Dictionary<string, HashSet<ushort>>();
            foreach (var entry in countriesDict)
            {
                if (!ushort.TryParse(entry.Key, out var parsedCountryId))
                {
                    continue;
                }
                try
                {
                    obj = entry.Value;
                    if (obj.ValueKind != JsonValueKind.Array)
                    {
                        continue;
                    }
                    var operators = obj.Deserialize<string[]>() ?? Array.Empty<string>();
                    foreach (var opId in operators)
                    {
                        if (!countryOperators.ContainsKey(opId))
                        {
                            countryOperators.Add(opId, new HashSet<ushort>());
                        }
                        if (countryOperators[opId]?.Contains(parsedCountryId) == true)
                        {
                            continue;
                        }
                        countryOperators[opId]?.Add(parsedCountryId);
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Trace.WriteLine(ex);
#endif
                }
            }
            var res = new Dictionary<string, SAOperator>();
            foreach (var entry in countryOperators)
            {
                var countries = new Dictionary<ushort, string?>();
                foreach (var country in entry.Value)
                {
                    countries[country] = null;
                }
                var parsed = new SAOperator(entry.Key, entry.Key, countries);
                res[parsed.ID] = parsed;
            }
            return new Result<Dictionary<string, SAOperator>, ApiError>(res);
        }

        internal static async Task<Result<Dictionary<string, SAActivationService>, ApiError>> GetServices()
        {
            var target = ApiTarget.Services;
            var request = Client.BuildRequest(target);
            var response = await GlobalEnv.ApiClient.RequestAsGenResponse(request);
            var err = response.Error;
            if (err != null)
            {
                return new Result<Dictionary<string, SAActivationService>, ApiError>(err);
            }
            var dat = response.Data;
            if (string.IsNullOrEmpty(dat))
            {
                return new Result<Dictionary<string, SAActivationService>, ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            }
            var dict = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(dat) ?? new Dictionary<string, dynamic>();
            if (!dict.ContainsKey("data"))
            {
                return new Result<Dictionary<string, SAActivationService>, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Not found 'data' key"));
            }
            JsonElement obj = dict["data"];
            if (obj.ValueKind != JsonValueKind.Array)
            {
                return new Result<Dictionary<string, SAActivationService>, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "'data' is not an array"));
            }
            var arr = obj.Deserialize<Dictionary<string, dynamic>[]>() ?? Array.Empty<Dictionary<string, dynamic>>();
            var res = new Dictionary<string, SAActivationService>();
            foreach (var itemDict in arr)
            {
                var parsed = SAActivationService.From(itemDict);
                if (parsed == null)
                {
#if DEBUG
                    Trace.WriteLine("Invalid JSON");
                    Trace.WriteLine(itemDict);
#endif
                    continue;
                }
                if (res.ContainsKey(parsed.ID))
                {
#if DEBUG
                    Trace.WriteLine("ID " + parsed.ID + " already contains in dictionary");
#endif
                    continue;
                }
                res.Add(parsed.ID, parsed);
            }
            return new Result<Dictionary<string, SAActivationService>, ApiError>(res);
        }

        internal static async Task<Result<Dictionary<ushort, SAActivationServiceOfferV2>, ApiError>> GetCountriesForActivationServiceV2(string serviceId, int forward)
        {
            var target = ApiTarget.ActivationServiceCountryOffersV2;
            var request = Client.BuildRequest(target, additionalReqParameters: new Dictionary<string, dynamic>
            {
                ["service"] = serviceId,
                ["forward"] = forward
            });
            var response = await GlobalEnv.ApiClient.RequestAsGenResponse(request);
            var err = response.Error;
            if (err != null)
            {
                return new Result<Dictionary<ushort, SAActivationServiceOfferV2>, ApiError>(err);
            }
            var dat = response.Data;
            if (string.IsNullOrEmpty(dat))
            {
                return new Result<Dictionary<ushort, SAActivationServiceOfferV2>, ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            }
            var dict = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(dat) ?? new Dictionary<string, dynamic>();
            if (!dict.ContainsKey("data"))
            {
                return new Result<Dictionary<ushort, SAActivationServiceOfferV2>, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Not found 'data' key"));
            }
            JsonElement obj = dict["data"];
            dict = obj.Deserialize<Dictionary<string, dynamic>>() ?? new Dictionary<string, dynamic>();
            if (!dict.ContainsKey("cards"))
            {
                return new Result<Dictionary<ushort, SAActivationServiceOfferV2>, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Not found 'cards' key"));
            }
            obj = dict["cards"];
            if (obj.ValueKind != JsonValueKind.Array)
            {
                return new Result<Dictionary<ushort, SAActivationServiceOfferV2>, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "'cards' is not an array"));
            }
            var arr = obj.Deserialize<Dictionary<string, dynamic>[]>() ?? Array.Empty<Dictionary<string, dynamic>>();
            var res = new Dictionary<ushort, SAActivationServiceOfferV2>();
            foreach (var itemDict in arr)
            {
                var parsed = SAActivationServiceOfferV2.From(itemDict, serviceId);
                if (parsed == null)
                {
#if DEBUG
                    Trace.WriteLine("Invalid JSON");
                    Trace.WriteLine(itemDict);
#endif
                    continue;
                }
                if (res.ContainsKey(parsed.CountryID))
                {
#if DEBUG
                    Trace.WriteLine("Country ID " + parsed.CountryID + " already contains in dictionary");
#endif
                    continue;
                }
                res.Add(parsed.CountryID, parsed);
            }
            return new Result<Dictionary<ushort, SAActivationServiceOfferV2>, ApiError>(res);
        }

        internal static async Task<Result<Dictionary<ushort, SAActivationServiceOfferV3>, ApiError>> GetCountriesForAcrivationServiceV3(string serviceId, int forward, bool freePrice)
        {
            var target = ApiTarget.ActivationServiceCountryOffersV3;
            var request = Client.BuildRequest(target, additionalReqParameters: new Dictionary<string, dynamic>
            {
                ["service"] = serviceId,
                ["forward"] = forward,
                ["freePrice"] = freePrice
            });
            var response = await GlobalEnv.ApiClient.RequestAsGenResponse(request);
            var err = response.Error;
            if (err != null)
            {
                return new Result<Dictionary<ushort, SAActivationServiceOfferV3>, ApiError>(err);
            }
            var dat = response.Data;
            if (string.IsNullOrEmpty(dat))
            {
                return new Result<Dictionary<ushort, SAActivationServiceOfferV3>, ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            }
            var dict = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, dynamic>>>(dat) ?? new Dictionary<string, Dictionary<string, dynamic>>();
            var res = new Dictionary<ushort, SAActivationServiceOfferV3>();
            foreach (var entry in dict)
            {
                if (!ushort.TryParse(entry.Key, out var countryId))
                {
#if DEBUG
                    Trace.WriteLine("Country ID parse fail " + entry.Key);
#endif
                    continue;
                }
                var parsed = SAActivationServiceOfferV3.From(entry.Value, countryId, serviceId);
                if (parsed == null)
                {
#if DEBUG
                    Trace.WriteLine("Invalid JSON for ID " + entry.Key);
#endif
                    continue;
                }
                res.Add(countryId, parsed);
            }
            return new Result<Dictionary<ushort, SAActivationServiceOfferV3>, ApiError>(res);
        }

        internal static async Task<Result<Dictionary<string, SARentServiceOffer>, ApiError>> GetRentServiceOffers(ushort rentHours, ushort? countryId)
        {
            var target = ApiTarget.RentServiceOffers;
            var reqParams = new Dictionary<string, dynamic>
            {
                ["timeValue"] = rentHours
            };
            if (countryId != null)
            {
                reqParams.Add("country", countryId);
            }
            var request = Client.BuildRequest(target, reqParams);
            
            var response = await GlobalEnv.ApiClient.RequestAsGenResponse(request);
            var err = response.Error;
            if (err != null)
            {
                return new Result<Dictionary<string, SARentServiceOffer>, ApiError>(err);
            }
            var dat = response.Data;
            if (string.IsNullOrEmpty(dat))
            {
                return new Result<Dictionary<string, SARentServiceOffer>, ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            }
            var dict = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(dat) ?? new Dictionary<string, dynamic>();
            if (!dict.ContainsKey("data"))
            {
                return new Result<Dictionary<string, SARentServiceOffer>, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Not found 'data' key"));
            }
            JsonElement obj = dict["data"];
            dict = obj.Deserialize<Dictionary<string, dynamic>>() ?? new Dictionary<string, dynamic>();
            if (!dict.ContainsKey("services"))
            {
                return new Result<Dictionary<string, SARentServiceOffer>, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Not found 'services' key"));
            }
            obj = dict["services"];
            if (obj.ValueKind == JsonValueKind.Array)
            {
                return new Result<Dictionary<string, SARentServiceOffer>, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "'services' is not an array"));
            }
            var innerDict = obj.Deserialize<Dictionary<string, Dictionary<string, dynamic>>>() ?? new Dictionary<string, Dictionary<string, dynamic>>();
            var res = new Dictionary<string, SARentServiceOffer>();
            foreach (var entry in innerDict)
            {
                var parsed = SARentServiceOffer.From(entry.Value, rentHours, countryId, entry.Key);
                if (parsed == null)
                {
#if DEBUG
                    Trace.WriteLine("Invalid JSON");
                    Trace.WriteLine(entry.Value);
#endif
                    continue;
                }
                res.Add(entry.Key, parsed);
            }
            return new Result<Dictionary<string, SARentServiceOffer>, ApiError>(res);
        }

        internal static async Task<Result<Dictionary<string, SARentServiceOffer>, ApiError>> GetRentServiceShortOffers(ushort rentHours, ushort? countryId, string? serviceId)
        {
            var target = ApiTarget.RentServiceShortOffers;
            var reqParams = new Dictionary<string, dynamic>
            {
                ["timeValue"] = rentHours
            };
            if (countryId != null)
            {
                reqParams.Add("country", countryId);
            }
            if (!string.IsNullOrEmpty(serviceId))
            {
                reqParams.Add("service", serviceId);
            }

            var request = Client.BuildRequest(target, reqParams);
            var response = await GlobalEnv.ApiClient.RequestAsGenResponse(request);
            var err = response.Error;
            if (err != null)
            {
                return new Result<Dictionary<string, SARentServiceOffer>, ApiError>(err);
            }
            var dat = response.Data;
            if (string.IsNullOrEmpty(dat))
            {
                return new Result<Dictionary<string, SARentServiceOffer>, ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            }
            var dict = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(dat) ?? new Dictionary<string, dynamic>();
            if (!dict.ContainsKey("data"))
            {
                return new Result<Dictionary<string, SARentServiceOffer>, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Not found 'data' key"));
            }
            JsonElement obj = dict["data"];
            dict = obj.Deserialize<Dictionary<string, dynamic>>() ?? new Dictionary<string, dynamic>();
            if (!dict.ContainsKey("services"))
            {
                return new Result<Dictionary<string, SARentServiceOffer>, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Not found 'services' key"));
            }
            obj = dict["services"];
            if (obj.ValueKind == JsonValueKind.Array)
            {
                return new Result<Dictionary<string, SARentServiceOffer>, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "'services' is not an array"));
            }
            var innerDict = obj.Deserialize<Dictionary<string, Dictionary<string, dynamic>>>() ?? new Dictionary<string, Dictionary<string, dynamic>>();
            var res = new Dictionary<string, SARentServiceOffer>();
            foreach (var entry in innerDict)
            {
                var parsed = SARentServiceOffer.From(entry.Value, rentHours, countryId, entry.Key);
                if (parsed == null)
                {
#if DEBUG
                    Trace.WriteLine("Invalid JSON");
                    Trace.WriteLine(entry.Value);
#endif
                    continue;
                }
                res.Add(entry.Key, parsed);
            }
            return new Result<Dictionary<string, SARentServiceOffer>, ApiError>(res);
        }
    }
}
