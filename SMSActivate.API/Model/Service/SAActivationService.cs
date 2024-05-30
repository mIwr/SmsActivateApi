using SmsActivate.API.Util;
using System.Collections.Generic;
using System.Text.Json;

namespace SmsActivate.API.Model
{
    /// <summary>
    /// SMS Activate activation service info
    /// </summary>
    public class SAActivationService: SAService
    {
        /// <summary>
        /// Can forward the activated phone flag
        /// </summary>
        public readonly bool Forward;

        public SAActivationService(string id, string name, bool forward) : base(id, name)
        {
            Forward = forward;
        }

        /// <summary>
        /// Try parse the activation service info from API response dictionary
        /// </summary>
        /// <param name="jsonDict">API response dictionary</param>
        /// <returns>Optional activation service info instance</returns>
        public static SAActivationService? From(Dictionary<string, dynamic> jsonDict)
        {
            if (!jsonDict.ContainsKey("code") || !jsonDict.ContainsKey("name"))
            {
                return null;
            }
            JsonElement obj = jsonDict["code"];
            var id = obj.GetString();
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            obj = jsonDict["name"];
            var name = obj.GetString();
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            var forward = false;
            if (jsonDict.ContainsKey("f"))
            {
                obj = jsonDict["f"];
                ModelParseUtil.TryParseBool(obj, out forward);
            }

            return new SAActivationService(id, name, forward);
        }
        //{"code":"tg","f":"0","name":"Telegram"}
    }
}
