using SmsActivate.API.Network;
using System;

namespace SmsActivate.API
{
    /// <summary>
    /// SMS Activate API exception listener interface
    /// </summary>
    public interface IExceptionListener
    {
        /// <summary>
        /// Handle the incoming API exception
        /// </summary>
        /// <param name="errorFromSrv">Raw API response from the back-end</param>
        /// <param name="parsed">Parsed API error</param>
        void Handle(string errorFromSrv, ApiError parsed);
    }

    /// <summary>
    /// API bans exception processor
    /// </summary>
    internal class ExceptionListenerImpl: IExceptionListener
    {
        /// <summary>
        /// Account temporary or permanent ban flag
        /// </summary>
        public bool Banned { get; private set; }
        /// <summary>
        /// Ban ends timestamp (UTC+0)
        /// </summary>
        public long BanUntilTsUTC { get; private set; }
        /// <summary>
        /// Ban ends local datetime
        /// </summary>
        public DateTime? BannedUntilLocal
        {
            get
            {
                if (!Banned || BanUntilTsUTC <= 0)
                {
                    return null;
                }
                var dtOffset = DateTimeOffset.FromUnixTimeSeconds(BanUntilTsUTC);
                var dt = dtOffset.ToLocalTime().DateTime;
                var dtNow = DateTime.Now;
                if (dtNow.CompareTo(dt) >= 0)
                {
                    Banned = false;
                    BanUntilTsUTC = 0;
                    return null;
                }
                return dt;
            }
        }

        internal ExceptionListenerImpl() { }

        public void Handle(string errorFromServer, ApiError parsed)
        {
            var nowTsUTC = DateTimeOffset.Now.ToUnixTimeSeconds();
            if (nowTsUTC >= BanUntilTsUTC)
            {
                Banned = false;
                BanUntilTsUTC = 0;
            }
            if (!parsed.BanError)
            {
                return;
            }            
            Banned = true;
            var banUntilLocal = parsed.BanUntil;
            if (banUntilLocal == null)
            {
                if (nowTsUTC >= BanUntilTsUTC)
                {
                    Banned = false;
                    BanUntilTsUTC = 0;
                }
                return;
            }
            var dtOffset = new DateTimeOffset(banUntilLocal.Value);
            var tsUTC = dtOffset.ToUniversalTime().ToUnixTimeSeconds();
            if (nowTsUTC >= tsUTC)
            {
                Banned = false;
                BanUntilTsUTC = 0;
                return;
            }
            BanUntilTsUTC = tsUTC;
        }
    }
}
