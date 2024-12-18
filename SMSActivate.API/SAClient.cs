﻿using SmsActivate.API.Model;
using SmsActivate.API.Network;
using System;
using System.Collections.Generic;

namespace SmsActivate.API
{
    /// <summary>
    /// Represents SMS Activate client controller
    /// </summary>
    public partial class SAClient: IDisposable
    {        
        /// <summary>
        /// Low-level API ban handler
        /// </summary>
        private readonly ExceptionListenerImpl _llBanListener;
        private readonly string _llBanListenerId;
        private readonly Client _llClient;
        /// <summary>
        /// SMS Activate API auth token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Available services for activation
        /// </summary>
        public Dictionary<string, SAActivationService> CachedActivationServicesInfo { get; private set; }
        /// <summary>
        /// Available services for rent
        /// </summary>
        public Dictionary<string, SAService> CachedRentServicesInfo { get; private set; }
        /// <summary>
        /// Available countries
        /// </summary>
        public Dictionary<ushort, SACountry> CachedCountriesInfo { get; private set; }
        /// <summary>
        /// Available mobile operators
        /// </summary>
        public Dictionary<string, SAOperator> CachedOperatorsInfo { get; private set; }
        /// <summary>
        /// Activations history
        /// </summary>
        public Dictionary<long, SAActivation> Activations { get; private set; }
        /// <summary>
        /// Account temporary or permanent ban flag
        /// </summary>
        public bool Banned
        {
            get => _llBanListener.Banned;
        }
        /// <summary>
        /// Ban ends timestamp (UTC+0)
        /// </summary>
        public long BanUntilTsUTC
        {
            get => _llBanListener.BanUntilTsUTC;
        }

        /// <summary>
        /// Ban ends local datetime
        /// </summary>
        public DateTime? BannedUntilLocal
        {
            get => _llBanListener.BannedUntilLocal;
        }

        /// <summary>
        /// Generated from local activation history with active ones
        /// </summary>
        public Dictionary<long, SAActivation> ActiveActivations
        {
            get
            {
                var res = new Dictionary<long, SAActivation>();
                foreach (var entry in Activations)
                {
                    if (entry.Value.Status == SAActivationStatus.OK || entry.Value.Status == SAActivationStatus.CANCEL)
                    {
                        continue;
                    }
                    res.Add(entry.Key, entry.Value);
                }
                return res;
            }
        }

        /// <summary>
        /// Initializes SMS Activate client
        /// </summary>
        /// <param name="token">SMS Activate API auth token</param>
        public SAClient(string token)
        {
            _llClient = new Client();
            _llBanListener = new();
            _llBanListenerId = _llClient.AddExceptionListener(_llBanListener);
            Token = token;
            CachedActivationServicesInfo = new Dictionary<string, SAActivationService>();
            CachedRentServicesInfo = new Dictionary<string, SAService>();
            CachedCountriesInfo = new Dictionary<ushort, SACountry>();
            CachedOperatorsInfo = new Dictionary<string, SAOperator>();
            Activations = new Dictionary<long, SAActivation>();
        }

        /// <summary>
        /// Sets low-level client host name
        /// </summary>
        /// <param name="host">Client host name</param>
        /// <returns>True - success set, otherwise - false</returns>
        public bool SetClientHost(string host)
        {
            return _llClient.SetHost(host);
        }

        /// <summary>
        /// Sets low-level client API host name
        /// </summary>
        /// <param name="host">Client API host name</param>
        /// <returns>True - success set, otherwise - false</returns>
        public bool SetClientApiHost(string host)
        {
            return _llClient.SetApiHost(host);
        }

        /// <summary>
        /// Disposes the client and linked resources
        /// </summary>
        public void Dispose()
        {
            _llClient.RemoveExceptionListener(_llBanListenerId);
            Token = string.Empty;
            CachedCountriesInfo.Clear();
            CachedOperatorsInfo.Clear();
            CachedActivationServicesInfo.Clear();
            CachedRentServicesInfo.Clear();
            Activations.Clear();           
            GC.SuppressFinalize(this);
        }
    }
}
