using SmsActivate.API.Model;
using SmsActivate.API.Util;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmsActivate.API.Network
{
    internal static class ApiProfile
    {
        private const string REGEX_PATTERN = "\\d+(?:[\\.,]\\d+)?";

        internal static async Task<Result<double, ApiError>> GetProfileBalance(string token)
        {
            var target = ApiTarget.ProfileBalance;
            var request = Client.BuildRequest(target, additionalReqParameters: new Dictionary<string, dynamic>
            {
                ["api_key"] = token,
            });
            var response = await GlobalEnv.ApiClient.RequestAsGenResponse(request);
            var err = response.Error;
            if (err != null)
            {
                return new Result<double, ApiError>(err);
            }
            var dat = response.Data;
            if (string.IsNullOrEmpty(dat))
            {
                return new Result<double, ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            }
            var matched = Regex.IsMatch(dat, REGEX_PATTERN);
            if (!matched)
            {
                return new Result<double, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Balance response regex match fail " + dat));
            }
            var group = Regex.Match(dat, REGEX_PATTERN).Groups[0];
            if (!double.TryParse(group.Value?.Replace('.',','), out var balance))
            {
                return new Result<double, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Account balance parse fail '" + dat + '\''));
            }
            return new Result<double, ApiError>(balance);
        }

        internal static async Task<Result<SAProfileBalance, ApiError>> GetProfileBalanceAndCashBack(string token)
        {
            var balanceRes = await GetProfileBalance(token);
            var balance = balanceRes.Data;
            var err = balanceRes.Error;
            if (err != null)
            {
                return new Result<SAProfileBalance, ApiError>(err);
            }
            var target = ApiTarget.ProfileBalanceAndCashBack;
            var request = Client.BuildRequest(target, additionalReqParameters: new Dictionary<string, dynamic>
            {
                ["api_key"] = token,
            });
            var response = await GlobalEnv.ApiClient.RequestAsGenResponse(request);
            err = response.Error;
            if (err != null)
            {
                return new Result<SAProfileBalance, ApiError>(err);
            }
            var dat = response.Data;
            if (string.IsNullOrEmpty(dat))
            {
                return new Result<SAProfileBalance, ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            }
            var matched = Regex.IsMatch(dat, REGEX_PATTERN);
            if (!matched)
            {
                return new Result<SAProfileBalance, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Cashback response regex match fail " + dat));
            }
            var group = Regex.Match(dat, REGEX_PATTERN).Groups[0];
            if (!double.TryParse(group.Value?.Replace('.', ','), out var balanceAndCashback))
            {
                return new Result<SAProfileBalance, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Account balance+cashback value parse fail '" + dat + '\''));
            }
            var res = new SAProfileBalance(balanceAndCashback - balance, balance);

            return new Result<SAProfileBalance, ApiError>(res);
        }

        internal static async Task<Result<SAProfileBalance, ApiError>> GetProfileBalanceAndCashBackV2(string token)
        {
            var target = ApiTarget.ProfileBalanceAndCashBackV2;
            var request = Client.BuildRequest(target, additionalReqParameters: new Dictionary<string, dynamic>
            {
                ["api_key"] = token,
            });
            var response = await GlobalEnv.ApiClient.RequestAsGenResponse(request);
            var err = response.Error;
            if (err != null)
            {
                return new Result<SAProfileBalance, ApiError>(err);
            }
            var dat = response.Data;
            if (string.IsNullOrEmpty(dat))
            {
                return new Result<SAProfileBalance, ApiError>(error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
            }
            var dict = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(dat) ?? new Dictionary<string, dynamic>();
            if (!dict.ContainsKey("balance"))
            {
                return new Result<SAProfileBalance, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Not found balance info"));
            }
            JsonElement obj = dict["balance"];
            if (!ModelParseUtil.TryParseDouble(obj, out var balance))
            {
                return new Result<SAProfileBalance, ApiError>(error: ApiErrorEnum.BadResponse.AsException(apiResponse: dat, message: "Balance parse fail from '" + obj.ToString() + '\''));
            }
            double cashback = 0.0;
            if (dict.ContainsKey("cashback"))
            {
                obj = dict["cashback"];
                ModelParseUtil.TryParseDouble(obj, out cashback);
            }
            return new Result<SAProfileBalance, ApiError>(data: new SAProfileBalance(cashback, balance));
        }
    }
}
