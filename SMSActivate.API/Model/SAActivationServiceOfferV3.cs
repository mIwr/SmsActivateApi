using SmsActivate.API.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace SmsActivate.API.Model
{
    /// <summary>
    ///  SMS Activate activation service offer for defined country (API version 3). Includes incoming call verification info
    /// </summary>
    public class SAActivationServiceOfferV3 : SAActivationServiceOffer
    {
        /// <summary>
        /// Default verified activation cost (retail price)
        /// </summary>
        public readonly double RetailVerifiedPrice;
        /// <summary>
        /// Default verified activation cost (incoming call activation)
        /// </summary>
        public readonly double VerifiedCallPrice;
        /// <summary>
        /// Default verified activation cost (incoming call activation)
        /// </summary>
        public readonly double RetailVerifiedCallPrice;

        /// <summary>
        /// Service supports rent on country
        /// </summary>
        public readonly bool Rent;
        /// <summary>
        /// Verified available activations (incoming call) count
        /// </summary>
        public uint VerifiedCallCount;

        public SAActivationServiceOfferV3(ushort countryID, string serviceID, uint total, uint count, double defaultPrice, double retailPrice, KeyValuePair<double, uint>[] prices, double retailVerifiedPrice, double verifiedCallPrice, double retailVerifiedCallPrice, bool rent, uint verifiedCallCount) : base(countryID, serviceID, total, count, defaultPrice, retailPrice, prices)
        {
            RetailVerifiedPrice = retailVerifiedPrice;
            VerifiedCallPrice = verifiedCallPrice;
            RetailVerifiedCallPrice = retailVerifiedCallPrice;
            Rent = rent;
            VerifiedCallCount = verifiedCallCount;
        }

        /// <summary>
        /// Try parse activation offers info instance from API response dictionary
        /// </summary>
        /// <param name="jsonDict">API response dictionary</param>
        /// <param name="serviceId">Service short name (ID)</param>
        /// <param name="countryId">Country ID</param>
        /// <returns>Optional activation offers info</returns>
        public static SAActivationServiceOfferV3? From(Dictionary<string, dynamic> jsonDict, ushort countryId, string serviceId)
        {
            if (!jsonDict.ContainsKey("totalCount") || !jsonDict.ContainsKey("retailPrice"))
            {
                return null;
            }
            JsonElement obj = jsonDict["totalCount"];
            uint totalCount = 0;
            if (obj.ValueKind == JsonValueKind.Number)
            {
                if (!obj.TryGetUInt32(out totalCount))
                {
                    totalCount = 0;
                }
            }
            else if (obj.ValueKind == JsonValueKind.String)
            {
                var str = obj.GetString();
                if (string.IsNullOrEmpty(str) || !uint.TryParse(str, out totalCount))
                {
                    totalCount = 0;
                }
            }
            obj = jsonDict["retailPrice"];
            if (!ModelParseUtil.TryParseDouble(obj, out var retailPrice))
            {
                retailPrice = 0.0;
            }
            double defaultPrice = 0.0;
            if (jsonDict.ContainsKey("defautPrice"))
            {
                obj = jsonDict["defautPrice"];
                ModelParseUtil.TryParseDouble(obj, out defaultPrice);
            }
            double verifiedPrice = 0.0;
            if (jsonDict.ContainsKey("verifiedPrice"))
            {
                obj = jsonDict["verifiedPrice"];
                ModelParseUtil.TryParseDouble(obj, out verifiedPrice);
            }
            double retailVerifiedPrice = 0.0;
            if (jsonDict.ContainsKey("retailVerifiedPrice"))
            {
                obj = jsonDict["retailVerifiedPrice"];
                ModelParseUtil.TryParseDouble(obj, out retailVerifiedPrice);
            }
            double verifiedCallPrice = 0.0;
            if (jsonDict.ContainsKey("verifiedCallPrice"))
            {
                obj = jsonDict["verifiedCallPrice"];
                ModelParseUtil.TryParseDouble(obj, out verifiedCallPrice);
            }
            double retailVerifiedCallPrice = 0.0;
            if (jsonDict.ContainsKey("retailVerifiedCallPrice"))
            {
                obj = jsonDict["retailVerifiedCallPrice"];
                ModelParseUtil.TryParseDouble(obj, out retailVerifiedCallPrice);
            }
            uint verifiedCount = 0;
            if (jsonDict.ContainsKey("verifiedCount"))
            {
                obj = jsonDict["verifiedCount"];
                ModelParseUtil.TryParseUInt32(obj, out verifiedCount);
            }
            uint verifiedCallCount = 0;
            if (jsonDict.ContainsKey("verifiedCallCount"))
            {
                obj = jsonDict["verifiedCallCount"];
                ModelParseUtil.TryParseUInt32(obj, out verifiedCallCount);
            }
            var rent = false;
            if (jsonDict.ContainsKey("rent"))
            {
                obj = jsonDict["rent"];
                ModelParseUtil.TryParseBool(obj, out rent);
            }
            var prices = new List<KeyValuePair<double, uint>>();
            if (jsonDict.ContainsKey("priceMap"))
            {
                obj = jsonDict["priceMap"];
                if (obj.ValueKind != JsonValueKind.Array)
                {
                    var map = obj.Deserialize<Dictionary<string, uint>>() ?? new Dictionary<string, uint>();
                    foreach (var entry in map)
                    {
                        if (!double.TryParse(entry.Key.Replace('.', ','), out var price))
                        {
                            if (!uint.TryParse(entry.Key, out var priceUL))
                            {
#if DEBUG
                                Trace.WriteLine("Incorrect price format in pair (" + entry.Key + ';' + entry.Value.ToString() + ')');
#endif
                                continue;
                            }
                            price = Convert.ToDouble(priceUL);
                        }
                        prices.Add(new KeyValuePair<double, uint>(price, entry.Value));
                    }
                    prices.Sort(comparison: new Comparison<KeyValuePair<double, uint>>((a, b) =>
                    {
                        return a.Key.CompareTo(b.Key);
                    }));
                }
            }


            return new SAActivationServiceOfferV3(countryId, serviceId, totalCount, verifiedCount, defaultPrice, retailPrice, prices.ToArray(), retailVerifiedPrice, verifiedCallPrice, retailVerifiedCallPrice, rent, verifiedCallCount);
            //{"totalCount":30200,"priceMap":{"12.87":35,"13.24":824,"13.29":87,"13.32":65,"13.44":33,"14.00":30,"14.25":32,"14.65":198,"14.67":94,"14.76":66,"15.00":427,"15.88":11,"15.95":102,"15.96":180,"15.97":280,"15.99":434,"16.00":1397,"16.16":25,"17.33":87,"18.67":119,"18.80":17,"20.00":36,"24.00":7,"25.33":64,"26.53":120,"26.73":192,"30.11":73,"32.00":9,"33.33":25000,"40.00":156},"defaultPrice":15,"rent":1,"verifiedCount":413,"verifiedCallCount":96,"retailPrice":22.5,"verifiedPrice":25,"retailVerifiedPrice":37.5,"verifiedCallPrice":25,"retailVerifiedCallPrice":37.5}
        }
    }
}