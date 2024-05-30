using System;

namespace SmsActivate.API.Network
{
    public partial class ApiError
    {
        /// <summary>
        /// API error caused by ban flag
        /// </summary>
        public bool BanError
        {
            get
            {
                return StockApiErrResponse.StartsWith(ApiErrorExt.BannedKey);
            }
        }

        /// <summary>
        /// Ban until datetime
        /// If it isn't a ban error or not temporary, returns null
        /// </summary>
        public DateTime? BanUntil
        {
            get
            {
                var banned = BanError;
                if (!banned)
                {
                    return null;
                }
                var index = StockApiErrResponse.IndexOf(':');
                if (index < 0 || index >= StockApiErrResponse.Length - 1)
                {
                    return null;
                }
                var dtStr = StockApiErrResponse.Substring(index + 1);
                if (!DateTime.TryParse(dtStr, out var dt))
                {
                    return null;
                }
                return dt;
            }
        }
    }
}
