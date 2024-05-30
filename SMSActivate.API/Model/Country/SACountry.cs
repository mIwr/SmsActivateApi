using SmsActivate.API.Util;
using System.Collections.Generic;
using System.Text.Json;
using System.Xml.Linq;

namespace SmsActivate.API.Model
{
    /// <summary>
    /// SMS Activate country info
    /// </summary>
    public class SACountry
    {
        /// <summary>
        /// Country ID
        /// </summary>
        public readonly ushort ID;
        /// <summary>
        /// Country russian name
        /// </summary>
        public readonly string NameRus;
        /// <summary>
        /// Country english name
        /// </summary>
        public readonly string NameEn;
        /// <summary>
        /// Country chinesse name
        /// </summary>
        public readonly string NameChn;
        /// <summary>
        /// Country is visible in list
        /// </summary>
        public readonly bool Visible;
        public readonly bool Retry;
        /// <summary>
        /// Country supports rent flag
        /// </summary>
        public readonly bool Rent;
        /// <summary>
        /// Country supports multi-service flag
        /// </summary>
        public readonly bool MultiService;

        public SACountry(ushort id, string nameRus, string nameEn, string nameChn, bool visible, bool retry, bool rent, bool multiService)
        {
            ID = id;
            NameRus = nameRus;
            NameEn = nameEn;
            NameChn = nameChn;
            Visible = visible;
            Retry = retry;
            Rent = rent;
            MultiService = multiService;
        }

        public override string ToString()
        {
            return ID.ToString() + " - " + NameEn;
        }

        /// <summary>
        /// Try parse country instance from API response dictionary
        /// </summary>
        /// <param name="jsonDict">API response dictionary</param>
        /// <returns>Optional country instance</returns>
        public static SACountry? From(Dictionary<string, dynamic> jsonDict)
        {
            if (!jsonDict.ContainsKey("id") || (!jsonDict.ContainsKey("rus") && !jsonDict.ContainsKey("eng") && !jsonDict.ContainsKey("chn")))
            {
                return null;
            }
            JsonElement obj = jsonDict["id"];
            var id = obj.Deserialize<ushort>();
            string rus = string.Empty, eng = string.Empty, chn = string.Empty;
            if (jsonDict.ContainsKey("rus")) {
                obj = jsonDict["rus"];
                rus = obj.GetString() ?? string.Empty;
            }
            if (jsonDict.ContainsKey("eng"))
            {
                obj = jsonDict["eng"];
                eng = obj.GetString() ?? string.Empty;
            }
            if (jsonDict.ContainsKey("chn"))
            {
                obj = jsonDict["chn"];
                chn = obj.GetString() ?? string.Empty;
            }
            var visible = true;
            if (jsonDict.ContainsKey("visible"))
            {
                obj = jsonDict["visible"];
                ModelParseUtil.TryParseBool(obj, out visible);
            }
            var retry = false;
            if (jsonDict.ContainsKey("retry"))
            {
                obj = jsonDict["retry"];
                ModelParseUtil.TryParseBool(obj, out retry);                
            }
            var rent = false;
            if (jsonDict.ContainsKey("rent"))
            {
                obj = jsonDict["rent"];
                ModelParseUtil.TryParseBool(obj, out rent);
            }
            var multiService = false;
            if (jsonDict.ContainsKey("multiService"))
            {
                obj = jsonDict["multiService"];
                ModelParseUtil.TryParseBool(obj, out multiService);
            }

            return new SACountry(id, rus, eng, chn, visible, retry, rent, multiService);
        }
    }
}
