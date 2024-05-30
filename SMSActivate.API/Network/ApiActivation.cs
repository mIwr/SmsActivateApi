using SmsActivate.API.Model;
using SmsActivate.API.Model.Batch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmsActivate.API.Network
{
    internal static class ApiActivation
    {
        internal const string ACCESS = "ACCESS";

        internal static async Task<Result<SAActivationDetailed[], ApiError>> GetProfileActiveActivations(string token)
        {
            var target = ApiTarget.ProfileActiveActivations;
            var request = Client.BuildRequest(target, additionalReqParameters: new Dictionary<string, dynamic>
            {
                ["api_key"] = token
            });
            var response = await GlobalEnv.ApiClient.RequestAsGenResponse(request);
            var err = response.Error;
            if (err != null)
            {
                if (err.ErrType == ApiErrorEnum.NoActivations)
                {
                    return new Result<SAActivationDetailed[], ApiError>(data: Array.Empty<SAActivationDetailed>());
                }
                return new Result<SAActivationDetailed[], ApiError>(err);
            }
            var dat = response.Data;
            if (string.IsNullOrEmpty(dat))
            {
                return new Result<SAActivationDetailed[], ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            }
            var dict = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(dat) ?? new Dictionary<string, dynamic>();
            if (!dict.ContainsKey("activeActivations"))
            {
                return new Result<SAActivationDetailed[], ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Not found 'activeActivations' key"));
            }
            JsonElement obj = dict["activeActivations"];
            if (obj.ValueKind != JsonValueKind.Array)
            {
                return new Result<SAActivationDetailed[], ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, "'activeActivations' key isn't an array"));
            }
            var arr = obj.Deserialize<Dictionary<string, dynamic>[]>() ?? Array.Empty<Dictionary<string, dynamic>>();
            var res = new List<SAActivationDetailed>();
            foreach (var itemDict in arr)
            {
                var parsed = SAActivationDetailed.From(itemDict);
                if (parsed == null)
                {
#if DEBUG
                    Trace.WriteLine("Invalid JSON");
                    Trace.WriteLine(itemDict);
#endif
                    continue;
                }
                res.Add(parsed);
            }
            return new Result<SAActivationDetailed[], ApiError>(res.ToArray());
        }

        internal static async Task<Result<SAActivationBatch, ApiError>> GetProfileActiveActivationsPaged(string token, uint start, uint length)
        {
            //&start=0&length=50&activationType=0
            var target = ApiTarget.ProfileActiveActivationsPaged;
            var request = Client.BuildRequest(target, additionalReqParameters: new Dictionary<string, dynamic>
            {
                ["api_key"] = token,
                ["start"] = start,
                ["length"] = length
            });
            var response = await GlobalEnv.ApiClient.RequestAsGenResponse(request);
            var err = response.Error;
            if (err != null)
            {
                return new Result<SAActivationBatch, ApiError>(err);
            }
            var dat = response.Data;
            if (string.IsNullOrEmpty(dat))
            {
                return new Result<SAActivationBatch, ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            }
            var dict = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(dat) ?? new Dictionary<string, dynamic>();
            var batch = SAActivationBatch.From(dict);
            return new Result<SAActivationBatch, ApiError>(data: batch);
        }

        internal static async Task<Result<SAActivation, ApiError>> RequestNewActivation(string token, string service, ushort country, double maxPrice, bool verificationCall, string[]? operatorIds, bool forward, bool useCashBack)
        {
            //service=$service&forward=$forward&operator=$operator&ref=$ref&country=$country&phoneException=$phoneException&maxPrice=maxPrice&verification=$verification&useCashBack=$useCashBack
            var target = ApiTarget.NewActivation;
            var reqParams = new Dictionary<string, dynamic>
            {
                ["api_key"] = token,
                ["service"] = service,
                ["country"] = country
            };
            if (maxPrice > 0.0)
            {
                reqParams["maxPrice"] = maxPrice;
            }
            if (verificationCall)
            {
                reqParams["verification"] = verificationCall;
            }
            if (operatorIds != null && operatorIds.Length > 0)
            {
                var operators = operatorIds[0];
                for (var i = 1; i < operators.Length; i++)
                {
                    operators += ',' + operatorIds[i];
                }
                reqParams["operator"] = operators;
            }
            if (forward)
            {
                reqParams["forward"] = forward;
            }
            if (useCashBack)
            {
                reqParams["useCashBack"] = useCashBack;
            }
            var request = Client.BuildRequest(target, reqParams);
            var response = await GlobalEnv.ApiClient.RequestAsGenResponse(request);
            var err = response.Error;
            if (err != null)
            {
                return new Result<SAActivation, ApiError>(err);
            }
            var dat = response.Data;
            if (string.IsNullOrEmpty(dat))
            {
                return new Result<SAActivation, ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            }
            if (!dat.StartsWith(ApiActivation.ACCESS))
            {
                return new Result<SAActivation, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Response doesn't start from '" + ApiActivation.ACCESS + "' keyword"));
            }
            string[] parts = dat.Split(':');
            if (parts.Length < 2)
            {
                return new Result<SAActivation, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Invalid response split parts from '" + dat + '\''));
            }
            if (!long.TryParse(parts[1], out var id))
            {
                return new Result<SAActivation, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Unable to parse activation ID from '" + parts[1] + "'. API response: " + dat));
            }
            if (!ulong.TryParse(parts[2], out var phone))
            {
                return new Result<SAActivation, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Unable to parse activation phone number from '" + parts[2] + "'. API response: " + dat));
            }

            return new Result<SAActivation, ApiError>(data: new SAActivation(id, phone, SAActivationStatus.WAIT_CODE, country, service, canGetAnotherSms: false, cost: 0.0, createTsUTC: DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
        }        

        internal static async Task<Result<SAActivation, ApiError>> RequestNewActivationV2(string token, string service, ushort country, double maxPrice, bool verificationCall, string[]? operatorIds, bool forward)
        {
            //&forward=$forward&operator=$operator&ref=$ref&phoneException=$phoneException&maxPrice=maxPrice&verification=$verification
            var target = ApiTarget.NewActivationV2;
            var reqParams = new Dictionary<string, dynamic>
            {
                ["api_key"] = token,
                ["service"] = service,
                ["country"] = country
            };
            if (maxPrice > 0.0)
            {
                reqParams["maxPrice"] = maxPrice;
            }
            if (verificationCall)
            {
                reqParams["verification"] = verificationCall;
            }
            if (operatorIds != null && operatorIds.Length > 0)
            {
                var operators = operatorIds[0];
                for (var i = 1; i < operators.Length; i++)
                {
                    operators += ',' + operatorIds[i];
                }
                reqParams["operator"] = operators;
            }
            if (forward)
            {
                reqParams["forward"] = forward;
            }
            var request = Client.BuildRequest(target, reqParams);            
            var response = await GlobalEnv.ApiClient.RequestAsGenResponse(request);
            var err = response.Error;
            if (err != null)
            {
                return new Result<SAActivation, ApiError>(err);
            }
            var dat = response.Data;
            if (string.IsNullOrEmpty(dat))
            {
                return new Result<SAActivation, ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            }
            var dict = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(dat) ?? new Dictionary<string, dynamic>();
            var parsed = SAActivation.From(dict, country, service);
            if (parsed == null)
            {
                return new Result<SAActivation, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Unable to parse activation info"));
            }
            return new Result<SAActivation, ApiError>(new SAActivation(parsed.ID, parsed.Phone, status: SAActivationStatus.WAIT_CODE, country, service, parsed.CanGetAnotherSMS, parsed.Cost, parsed.CreateTsUTC));
        }

        internal static async Task<Result<KeyValuePair<SAActivationStatus, string?>, ApiError>> GetActivationStatusWithSms(string token, long activationId)
        {
            var target = ApiTarget.ActivationStatus;
            var request = Client.BuildRequest(target, additionalReqParameters: new Dictionary<string, dynamic>
            {
                ["api_key"] = token,
                ["id"] = activationId
            });
            var response = await GlobalEnv.ApiClient.RequestAsGenResponse(request);
            var err = response.Error;
            if (err != null)
            {
                return new Result<KeyValuePair<SAActivationStatus, string?>, ApiError>(err);
            }
            var dat = response.Data;
            if (string.IsNullOrEmpty(dat))
            {
                return new Result<KeyValuePair<SAActivationStatus, string?>, ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            }
            SAActivationStatus? status;
            string? code = null;
            if (dat.Contains(':'))
            {
                var parts = dat.Split(':');
                status = SAActivationStatusExt.From(parts[0]);
                code = parts[1];
                if (code == "Нет кода")
                {
                    code = null;
                }
            }
            else
            {
                status = SAActivationStatusExt.From(dat);
            }            
            if (status == null)
            {
                return new Result<KeyValuePair<SAActivationStatus, string?>, ApiError>(error: ApiErrorEnum.BadStatus.AsException(apiResponse: dat, message: "Activation status parse fail " + dat));
            }
            return new Result<KeyValuePair<SAActivationStatus, string?>, ApiError>(data: new KeyValuePair<SAActivationStatus, string?>(status.Value, code));
        }

        internal static async Task<Result<SAActivationStatus, ApiError>> SetActivationStatus(string token, long activationId, byte newStatus)
        {
            var target = ApiTarget.ActivationStatusChange;
            var request = Client.BuildRequest(target, additionalReqParameters: new Dictionary<string, dynamic>
            {
                ["api_key"] = token,
                ["id"] = activationId,
                ["status"] = newStatus
            });
            var response = await GlobalEnv.ApiClient.RequestAsGenResponse(request);
            var err = response.Error;
            if (err != null)
            {
                return new Result<SAActivationStatus, ApiError>(err);
            }
            var dat = response.Data;
            if (string.IsNullOrEmpty(dat))
            {
                return new Result<SAActivationStatus, ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            }
            SAActivationStatus? status = null;
            var uppercased = dat.ToUpper();
            if (uppercased.Contains("ACCESS_READY"))
            {
                status = SAActivationStatus.WAIT_CODE;
            }
            if (uppercased.Contains("ACCESS_RETRY_GET"))
            {
                status = SAActivationStatus.WAIT_RESEND;
            }
            if (uppercased.Contains("ACCESS_ACTIVATION"))
            {
                status = SAActivationStatus.OK;
            }
            if (uppercased.Contains("ACCESS_CANCEL"))
            {
                status = SAActivationStatus.CANCEL;
            }
            if (status == null)
            {
                return new Result<SAActivationStatus, ApiError>(error: ApiErrorEnum.BadStatus.AsException(apiResponse: dat, message: "Server status parse fail " + dat));
            }
            return new Result<SAActivationStatus, ApiError>(status.Value);
        }
    }
}
