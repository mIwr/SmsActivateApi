using SmsActivate.API.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace SmsActivate.API.Model
{
    /// <summary>
    /// SMS Activate activation service offer for defined country (API version 2)
    /// </summary>
    public class SAActivationServiceOfferV2: SAActivationServiceOffer
    {
        /// <summary>
        /// Service supports 'Free Price' flag
        /// </summary>
        public readonly bool FreePrice;
        /// <summary>
        /// Min price for the activation
        /// </summary>
        public readonly double MinPrice;
        /// <summary>
        /// Max rank
        /// </summary>
        public readonly int MaxRank;

        public SAActivationServiceOfferV2(ushort countryID, string serviceID, uint total, uint count, double defaultPrice, double retailPrice, KeyValuePair<double, uint>[] prices, bool freePrice, double minPrice, int maxRank) :base(countryID, serviceID, total, count, defaultPrice, retailPrice, prices)
        {
            FreePrice = freePrice;
            MinPrice = minPrice;
            MaxRank = maxRank;
        }

        /// <summary>
        /// Try parse activation offers info instance from API response dictionary
        /// </summary>
        /// <param name="jsonDict">API response dictionary</param>
        /// <param name="serviceId">Service short name (ID)</param>
        /// <param name="countryId">Country ID. Optional</param>
        /// <returns>Optional activation offers info</returns>
        public static SAActivationServiceOfferV2? From(Dictionary<string, dynamic> jsonDict, string serviceId, string countryId = "")
        {
            if (!jsonDict.ContainsKey("totalCount") || !jsonDict.ContainsKey("prices"))
            {
                return null;
            }
            JsonElement obj;
            ushort country = 0;
            if (string.IsNullOrEmpty(countryId))
            {
                if (!jsonDict.ContainsKey("country"))
                {
                    return null;
                }
                obj = jsonDict["country"];
                if (!ModelParseUtil.TryParseUInt16(obj, out country))
                {
                    return null;
                }
            }
            obj = jsonDict["prices"];
            var pricesMap = obj.Deserialize<Dictionary<string, dynamic>>() ?? new Dictionary<string, dynamic>();
            if (!pricesMap.ContainsKey("retail"))
            {
                return null;
            }
            JsonElement innerObj = pricesMap["retail"];
            if (!ModelParseUtil.TryParseDouble(innerObj, out var retailPrice))
            {
                return null;
            }
            double defaultPrice = 0.0;
            if (pricesMap.ContainsKey("def"))
            {
                innerObj = pricesMap["def"];
                if (!ModelParseUtil.TryParseDouble(innerObj, out defaultPrice))
                {
                    defaultPrice = retailPrice;
                }
            }
            double minPrice = 0.0;
            if (pricesMap.ContainsKey("min"))
            {
                innerObj = pricesMap["min"];
                if (!ModelParseUtil.TryParseDouble(innerObj, out minPrice))
                {
                    minPrice = retailPrice;
                }
            }
            int maxRank = 0;
            if (pricesMap.ContainsKey("maxRank"))
            {
                innerObj = pricesMap["maxRank"];
                ModelParseUtil.TryParseInt32(innerObj, out maxRank);
            }
            obj = jsonDict["totalCount"];
            uint totalCount = 0;
            if (obj.ValueKind == JsonValueKind.Number)
            {
                if (!obj.TryGetUInt32(out totalCount))
                {
                    totalCount = 0;
                }
            }
            uint count = 0;
            if (jsonDict.ContainsKey("count"))
            {
                obj = jsonDict["count"];
                ModelParseUtil.TryParseUInt32(obj, out count);
            }
            var freePrice = false;
            if (jsonDict.ContainsKey("freePrice"))
            {
                obj = jsonDict["freePrice"];
                ModelParseUtil.TryParseBool(obj, out freePrice);
            }

            var prices = new List<KeyValuePair<double, uint>>();
            if (jsonDict.ContainsKey("offers"))
            {
                obj = jsonDict["offers"];
                if (obj.ValueKind != JsonValueKind.Array)
                {
                    //Non-empty
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

            return new SAActivationServiceOfferV2(country, serviceId, totalCount, count, defaultPrice, retailPrice, prices.ToArray(), freePrice, minPrice, maxRank);
        }
        //{"totalCount":28150,"count":28150,"freePrice":true,"offers":{"40.00":4,"37.33":3,"34.67":86,"33.33":25000,"30.11":52,"26.67":229,"26.53":120,"24.00":12,"20.00":68,"18.80":14,"18.67":251,"17.89":57,"17.33":86,"16.00":977,"15.00":208,"14.65":330,"14.64":196,"14.63":124,"14.61":42,"14.52":4,"14.49":270,"14.45":5,"12.96":12},"prices":{"def":15,"retail":22.5,"min":22.5,"maxRank":9},"country":6}       
    }
}
