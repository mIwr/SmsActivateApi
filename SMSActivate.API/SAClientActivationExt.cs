using SmsActivate.API.Model;
using SmsActivate.API.Model.Batch;
using SmsActivate.API.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SmsActivate.API
{
    public partial class SAClient
    {
        /// <summary>
        /// Get all account active activations
        /// </summary>
        /// <returns>API response result with the account active activations</returns>
        public async Task<Result<SAActivationDetailed[], ApiError>> GetProfileActiveActivations()
        {
            var res = await ApiActivation.GetProfileActiveActivations(Token);
            var activations = res.Data;
            if (activations != null)
            {
                if (activations.Length == 0)
                {
                    return res;
                }
                foreach (var activation in activations)
                {
                    if (!Activations.ContainsKey(activation.ID))
                    {
                        Activations.Add(activation.ID, activation);
                        continue;
                    }
                    Activations[activation.ID] = activation;
                }                
            }
            return res;
        }

        /// <summary>
        /// Get paged account active activations (Mobile API)
        /// </summary>
        /// <param name="start">Page start index. Default value is 0</param>
        /// <param name="length">Page size. Default value is 50</param>
        /// <returns>API response result with the account active activations batch</returns>
        public async Task<Result<SAActivationBatch, ApiError>> GetProfileActiveActivationsPaged(uint start = 0, uint length = 50)
        {
            var res = await ApiActivation.GetProfileActiveActivationsPaged(Token, start, length);
            var batch = res.Data;
            if (batch != null)
            {
                foreach (var activation in batch.Data)
                {
                    if (!Activations.ContainsKey(activation.ID))
                    {
                        Activations.Add(activation.ID, activation);
                        continue;
                    }
                    Activations[activation.ID] = activation;
                }
            }
            return res;
        }

#if DEBUG
        /// <summary>
        /// For testing and debug only
        /// Request the activation phone via legacy API
        /// </summary>
        /// <param name="serviceId">Service short name (ID)</param>
        /// <param name="countryId">Country ID</param>
        /// <param name="maxPrice">Activation max price (Free Price feature)</param>
        /// <param name="verificationCall">Request activation for incoming call</param>
        /// <param name="operatorIds">Allowed operator IDs</param>
        /// <param name="forward">Use forward</param>
        /// <param name="useCashBack">Use cashback for activation buy</param>
        /// <returns>API response result with the activation</returns>
        public async Task<Result<SAActivation, ApiError>> RequestLegacyNewActivation(string serviceId, ushort countryId, double maxPrice = 0.0, bool verificationCall = false, string[]? operatorIds = null, bool forward = false, bool useCashBack = false)
        {
            if (CachedActivationServicesInfo.Count > 0 && !CachedActivationServicesInfo.ContainsKey(serviceId))
            {
                return new Result<SAActivation, ApiError>(error: ApiErrorEnum.BadService.AsException(apiResponse: string.Empty));
            }
            if (CachedCountriesInfo.Count > 0 && !CachedCountriesInfo.ContainsKey(countryId))
            {
                return new Result<SAActivation, ApiError>(error: ApiErrorEnum.BadCountry.AsException(apiResponse: string.Empty));
            }
            var res = await ApiActivation.RequestNewActivation(Token, serviceId, countryId, maxPrice, verificationCall, operatorIds, forward, useCashBack);
            var activation = res.Data;
            if (activation != null)
            {
                Activations.Add(countryId, activation);
            }
            return res;
        }
#endif

        /// <summary>
        /// Request the activation phone
        /// </summary>
        /// <param name="serviceId">Service short name (ID)</param>
        /// <param name="countryId">Country ID</param>
        /// <param name="maxPrice">Activation max price (Free Price feature)</param>
        /// <param name="verificationCall">Request activation for incoming call</param>
        /// <param name="operatorIds">Allowed operator IDs</param>
        /// <param name="forward">Use forward</param>
        /// <param name="useCashBack">Use cashback for activation buy</param>
        /// <returns>API response result with the activation</returns>
        public async Task<Result<SAActivation, ApiError>> RequestNewActivation(string serviceId, ushort countryId, double maxPrice = 0.0, bool verificationCall = false, string[]? operatorIds = null, bool forward = false, bool useCashBack = false)
        {
            if (CachedActivationServicesInfo.Count > 0 && !CachedActivationServicesInfo.ContainsKey(serviceId))
            {
                return new Result<SAActivation, ApiError>(error: ApiErrorEnum.BadService.AsException(apiResponse: string.Empty));
            }
            if (CachedCountriesInfo.Count > 0 && !CachedCountriesInfo.ContainsKey(countryId))
            {
                return new Result<SAActivation, ApiError>(error: ApiErrorEnum.BadCountry.AsException(apiResponse: string.Empty));
            }
            var res = await ApiActivation.RequestNewActivationV2(Token, serviceId, countryId, maxPrice, verificationCall, operatorIds, forward);            
            var activation = res.Data;
            if (activation == null)
            {
#if DEBUG
                Trace.WriteLine("Request activation V2 fail");
                var err = res.Error;
                if (err != null)
                {
                    Trace.WriteLine(err);
                }
                Trace.WriteLine("Request activation in legacy mode");
#endif
                res = await ApiActivation.RequestNewActivation(Token, serviceId, countryId, maxPrice, verificationCall, operatorIds, forward, useCashBack);
                activation = res.Data;
                if (activation != null)
                {
                    Activations.Add(activation.ID, activation);
                }
            }
            else
            {
                Activations.Add(activation.ID, activation);
            }
            return res;
        }

        /// <summary>
        /// Get the activation status with SMS code
        /// </summary>
        /// <param name="activationId">Activation ID</param>
        /// <returns>API response result with the activation status and SMS code</returns>
        public async Task<Result<KeyValuePair<SAActivationStatus, string?>, ApiError>> GetActivationStatusWithSMS(long activationId)
        {
            var res = await ApiActivation.GetActivationStatusWithSms(Token, activationId);
            var pair = res.Data;
            if (res.Error == null && Activations.ContainsKey(activationId))
            {
                var activation = Activations[activationId];
                if (activation.Status != pair.Key)
                {
                    Activations[activationId] = new SAActivation(activationId, activation.Phone, pair.Key, activation.CountryID, activation.ServiceID, activation.CanGetAnotherSMS, activation.Cost, activation.CreateTsUTC);
                }
            }
            return res;
        }

        /// <summary>
        /// Wait SMS code for the activation by multiple 'GetActivationStatusWithSMS' calls
        /// </summary>
        /// <param name="activationId">Activation ID</param>
        /// <param name="waitIterationInS">'Try' duration in seconds</param>
        /// <param name="waitIterationCount">Tries count</param>
        /// <returns>API response result with the SMS code</returns>
        public async Task<Result<string, ApiError>> WaitSMS(long activationId, int waitIterationInS = 10, int waitIterationCount = 6)
        {
            var safeWaitIterationInS = waitIterationInS;
            if (safeWaitIterationInS <= 0)
            {
                safeWaitIterationInS = 10;
            }
            var safeIterationCount = waitIterationCount;
            if (safeIterationCount <= 0)
            {
                safeIterationCount = 1;
            }
            var iter = 0;
            var statusRes = new Result<KeyValuePair<SAActivationStatus, string?>, ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            while (iter < safeIterationCount)
            {
                statusRes = await GetActivationStatusWithSMS(activationId);
                var status = statusRes.Data.Key;
                var code = statusRes.Data.Value;
                if (status == SAActivationStatus.OK && !string.IsNullOrEmpty(code))
                {
                    return new Result<string, ApiError>(code);
                }
                if (status == SAActivationStatus.CANCEL)
                {
                    return new Result<string, ApiError>(error: ApiErrorEnum.AlreadyCancel.AsException(apiResponse: string.Empty));
                }
                iter++;
                if (iter == safeIterationCount)
                {
                    break;
                }
                await Task.Delay(safeWaitIterationInS * 1000);
            }
            return new Result<string, ApiError>(statusRes.Data.Value, statusRes.Error);
        }

        /// <summary>
        /// Wait SMS code for the activation by multiple 'GetProfileActiveActivations' calls
        /// </summary>
        /// <param name="activationId">Activation ID</param>
        /// <param name="waitIterationInS">'Try' duration in seconds</param>
        /// <param name="waitIterationCount">Tries count</param>
        /// <returns>API response result with the SMS code</returns>
        public async Task<Result<SAActivationDetailed, ApiError>> WaitSMSExperimental(long activationId, int waitIterationInS = 10, int waitIterationCount = 6)
        {
            var safeWaitIterationInS = waitIterationInS;
            if (safeWaitIterationInS <= 0)
            {
                safeWaitIterationInS = 10;
            }
            var safeIterationCount = waitIterationCount;
            if (safeIterationCount <= 0)
            {
                safeIterationCount = 1;
            }
            var iter = 0;
            var foundTargetActivation = false;
            while (iter < safeIterationCount)
            {
                var activationsRes = await GetProfileActiveActivations();
                var activations = activationsRes.Data;
                if (activations == null || activations.Length == 0)
                {
                    iter++;
                    if (iter == safeIterationCount)
                    {
                        break;
                    }
                    await Task.Delay(safeWaitIterationInS * 1000);                    
                    continue;
                }
                SAActivationDetailed? targetActivation = null;
                foreach (var activation in activations)
                {
                    if (activation.ID != activationId)
                    {
                        continue;
                    }
                    targetActivation = activation;
                    foundTargetActivation = true;
                    break;
                }
                if (targetActivation == null)
                {
                    return new Result<SAActivationDetailed, ApiError>(error: ApiErrorEnum.WrongActivationID.AsException(apiResponse: string.Empty, message: "Not found on active activations. Possible is cancelled, finished or expired"));
                }
                if (!string.IsNullOrEmpty(targetActivation.SmsCode))
                {
                    return new Result<SAActivationDetailed, ApiError>(data: targetActivation);
                }
                iter++;
                if (iter == safeIterationCount)
                {
                    break;
                }
                await Task.Delay(safeWaitIterationInS * 1000);
            }
            if (!foundTargetActivation)
            {
                return new Result<SAActivationDetailed, ApiError>(error: ApiErrorEnum.WrongActivationID.AsException(apiResponse: string.Empty, message: "Wait SMS timeout and not found on active activations. Possible is cancelled, finished or expired"));
            }
            return new Result<SAActivationDetailed, ApiError>(error: ApiErrorEnum.BadStatus.AsException(apiResponse: string.Empty, message: "Wait SMS timeout"));
        }

        /// <summary>
        /// Set 'cancel' activation status
        /// </summary>
        /// <param name="activationId">Activation ID</param>
        /// <returns>API response result with the new activation status</returns>
        public Task<Result<SAActivationStatus, ApiError>> ActivationCancel(long activationId)
        {
            return SetActivationStatus(activationId, SAActivationStatus.CANCEL.ApiCode());
        }

        /// <summary>
        /// Set 'SmsSent' activation status
        /// </summary>
        /// <param name="activationId">Activation ID</param>
        /// <returns>API response result with the new activation status</returns>
        public Task<Result<SAActivationStatus, ApiError>> ActivationNotifySmsSent(long activationId)
        {
            return SetActivationStatus(activationId, SAActivationStatusExt.NotifySmsSentApiCode);
        }

        /// <summary>
        /// Set 'SmsResend' activation status
        /// </summary>
        /// <param name="activationId">Activation ID</param>
        /// <returns>API response result with the new activation status</returns>
        public Task<Result<SAActivationStatus, ApiError>> ActivationResendSMS(long activationId)
        {
            return SetActivationStatus(activationId, SAActivationStatus.WAIT_RESEND.ApiCode());
        }

        /// <summary>
        /// Set 'OK' (Finish) activation status
        /// </summary>
        /// <param name="activationId">Activation ID</param>
        /// <returns>API response result with the new activation status</returns>
        public Task<Result<SAActivationStatus, ApiError>> ActivationFinish(long activationId)
        {
            return SetActivationStatus(activationId, SAActivationStatus.OK.ApiCode());
        }

        /// <summary>
        /// Set activation status
        /// </summary>
        /// <param name="activationId">Activation ID</param>
        /// <param name="apiCode">Activation status API code</param>
        /// <returns>API response result with the new activation status</returns>
        public async Task<Result<SAActivationStatus, ApiError>> SetActivationStatus(long activationId, byte apiCode)
        {
            var res = await ApiActivation.SetActivationStatus(Token, activationId, apiCode);
            var status = res.Data;
            if (Activations.ContainsKey(activationId))
            {
                var activation = Activations[activationId];
                var updActivaton = new SAActivation(activationId, activation.Phone, status, activation.CountryID, activation.ServiceID, activation.CanGetAnotherSMS, activation.Cost, activation.CreateTsUTC);
                if (activation.Status != status)
                {
                    if (res.Error != null)
                    {
                        if (res.Error.ErrType == ApiErrorEnum.AlreadyCancel)
                        {
                            Activations[activationId] = updActivaton;
                        }
                        else if (res.Error.ErrType == ApiErrorEnum.AlreadyFinish)
                        {
                            Activations[activationId] = updActivaton;
                        }
                    }
                    else
                    {
                        Activations[activationId] = updActivaton;
                    }
                }                
            }
            return res;
        }
    }
}
