using SmsActivate.API.Util;
using System.Collections.Generic;
using System.Text.Json;

namespace SmsActivate.API.Model
{
    public class SARentServiceOffer: SAService
    {
        /// <summary>
        /// Rent duration in hours
        /// </summary>
        public readonly ushort Hours;
        /// <summary>
        /// Available for rent count
        /// </summary>
        public readonly uint Count;
        /// <summary>
        /// Total rent count
        /// </summary>
        public readonly uint TotalCount;
        /// <summary>
        /// Base rent price
        /// </summary>
        public readonly double Price;
        /// <summary>
        /// Final retail rent price
        /// </summary>
        public readonly double RetailPrice;

        /// <summary>
        /// Origin country ID
        /// </summary>
        public readonly ushort? CountryId;

        /// <summary>
        /// Parsed the closest duration variant
        /// </summary>
        public SARentDuration ApiDuration
        {
            get
            {
                return SARentDurationExt.ClosestFromHours(Hours);
            }
        }

        public SARentServiceOffer(string id, string name, ushort? countryId, double price, double retailPrice, uint count, uint totalCount, ushort hours) : base(id, name)
        {
            CountryId = countryId;
            Price = price;
            RetailPrice = retailPrice;
            Count = count;
            TotalCount = totalCount;
            Hours = hours;
        }

        /// <summary>
        /// Try parse rent offer info instance from API response dictionary
        /// </summary>
        /// <param name="jsonDict">API response dictionary</param>
        /// <param name="hours">Contexual rent hours</param>
        /// <param name="countryId">Contextual country ID. Optional</param>
        /// <param name="serviceId">Contextual service ID. Optional</param>
        /// <param name="serviceName">Service name. Optional</param>
        /// <returns></returns>
        public static SARentServiceOffer? From(Dictionary<string, dynamic> jsonDict, ushort hours, ushort? countryId = null, string serviceId = "", string serviceName = "")
        {
            if (!jsonDict.ContainsKey("retail_cost") || !jsonDict.ContainsKey("quant"))
            {
                return null;
            }            
            JsonElement obj;            
            var id = serviceId;
            if (string.IsNullOrEmpty(serviceId) && jsonDict.ContainsKey("service"))
            {
                obj = jsonDict["service"];
                id = obj.GetString() ?? string.Empty;
            }
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            var name = serviceName;
            if (string.IsNullOrEmpty(name) && jsonDict.ContainsKey("name"))
            {
                obj = jsonDict["name"];
                name = obj.GetString() ?? string.Empty;
            }
            if (string.IsNullOrEmpty(name))
            {
                name = id;
            }
            obj = jsonDict["retail_cost"];
            if (!ModelParseUtil.TryParseDouble(obj, out var retailPrice))
            {
                retailPrice = 0;
            }
            double price = 0.0;
            if (jsonDict.ContainsKey("cost"))
            {
                obj = jsonDict["cost"];
                ModelParseUtil.TryParseDouble(obj, out price);
            }
            obj = jsonDict["quant"];
            var quantMap = obj.Deserialize<Dictionary<string, uint>>() ?? new Dictionary<string, uint>();
            if (!quantMap.ContainsKey("current"))
            {
                return null;
            }
            var currCount = quantMap["current"];
            var total = currCount;
            if (jsonDict.ContainsKey("total"))
            {
                total = quantMap["total"];
            }
            return new SARentServiceOffer(id, name, countryId, price, retailPrice, currCount, total, hours);
        }
        //{"name":"Полная аренда","cost":162,"retail_cost":"243","quant":{"current":4662,"total":4770},"search_name":"Полная арендаFull rentполная аренда,full rent,gjkyfz,агдд"}
    }
}
