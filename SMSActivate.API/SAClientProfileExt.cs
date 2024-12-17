using SmsActivate.API.Model;
using SmsActivate.API.Network;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SmsActivate.API
{
    public partial class SAClient
    {
        /// <summary>
        /// Get account balance
        /// </summary>
        /// <returns>API response result with the account balance info</returns>
        public async Task<Result<double, ApiError>> GetProfileBalance()
        {
            var res = await _llClient.GetProfileBalance(Token);
            return res;
        }

#if DEBUG
        /// <summary>
        /// For testing and debug only
        /// Gets account balance and cashback info via legacy API
        /// </summary>
        /// <returns>Account balance and cashback info</returns>
        public async Task<Result<SAProfileBalance, ApiError>> GetLegacyProfileBalanceAndCashback()
        {
            var res = await _llClient.GetProfileBalanceAndCashBack(Token);
            return res;
        }
#endif

        /// <summary>
        /// Gets account balance and cashback info
        /// </summary>
        /// <returns>Account balance and cashback info</returns>
        public async Task<Result<SAProfileBalance, ApiError>> GetProfileBalanceAndCashback()
        {
            var responseRes = await _llClient.GetProfileBalanceAndCashBackV2(Token);
            var err = responseRes.Error;
            if (err != null)
            {
#if DEBUG
                Trace.WriteLine("Get profile balance with cashback V2 fail");
                Trace.WriteLine(err);
                Trace.WriteLine("Get legacy profile balance with cashback");
#endif
                responseRes = await _llClient.GetProfileBalanceAndCashBack(Token);
            }

            return responseRes;
        }
    }
}
