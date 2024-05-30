using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace SmsActivate.API.Model
{
    /// <summary>
    /// SMS Activate mobile operator
    /// </summary>
    public class SAOperator
    {
        /// <summary>
        /// Operator ID (name)
        /// </summary>
        public readonly string ID;
        /// <summary>
        /// Operator name
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// Operator countries. Key is a country ID, value is a name (Optional)
        /// </summary>
        public readonly Dictionary<ushort, string?> Countries;
        /// <summary>
        /// Operator english name
        /// </summary>
        public readonly string? NameEn;
        /// <summary>
        /// Operator russian name
        /// </summary>
        public readonly string? NameRu;
        /// <summary>
        /// Operator chinesse name
        /// </summary>
        public readonly string? NameChn;

        public SAOperator(string id, string name, Dictionary<ushort, string?> countries, string? nameEn = null, string? nameRu = null, string? nameChn = null)
        {
            ID = id;
            Name = name;
            Countries = countries;

            NameEn = nameEn;
            NameRu = nameRu;
            NameChn = nameChn;
        }

        /// <summary>
        /// Try parse mobile operators in country from mobile API response dictionary
        /// </summary>
        /// <param name="jsonDict">API response dictionary</param>
        /// <param name="countryId">Country ID</param>
        /// <param name="countryName">Country name. Optional</param>
        /// <returns>Country operators list</returns>
        public static SAOperator[] FromMobileAPI(Dictionary<string, dynamic> jsonDict, ushort countryId, string? countryName = null) 
        {
            //"85":{"orange":{"ru":"Orange","en":"Orange","cn":"橙子"},"moldcell":{"ru":"Moldcell","en":"Moldcell","cn":"黴菌細胞"},"unite":{"ru":"Unite","en":"Unite","cn":"團結"},"idc":{"ru":"IDC","en":"IDC","cn":""}}
            var res = new List<SAOperator>();
            var countries = new Dictionary<ushort, string?>
            {
                [countryId] = countryName
            };
            foreach (var entry in jsonDict)
            {
                var parsed = new SAOperator(entry.Key, entry.Key, countries);
                try
                {
                    JsonElement el = entry.Value;
                    var operatorDict = el.Deserialize<Dictionary<string, dynamic>>() ?? new Dictionary<string, dynamic>();
                    string? nameRu = null, nameEn = null, nameChn = null;
                    if (operatorDict.ContainsKey("ru"))
                    {
                        el = operatorDict["ru"];
                        nameRu = el.GetString();
                    }
                    if (operatorDict.ContainsKey("en"))
                    {
                        el = operatorDict["en"];
                        nameEn = el.GetString() ?? entry.Key;
                    }
                    if (operatorDict.ContainsKey("cn"))
                    {
                        el = operatorDict["cn"];
                        nameChn = el.GetString();
                    }
                    parsed = new SAOperator(entry.Key, entry.Key, countries, nameEn, nameRu, nameChn);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Trace.WriteLine(ex);
#endif
                }
                res.Add(parsed);
            }            
            return res.ToArray();
        }

        /// <summary>
        /// Try parse mobile operator from web API response dictionary
        /// </summary>
        /// <param name="jsonDict">API response dictionary</param>
        /// <param name="id">Operator ID</param>
        /// <returns>Optional mobile operator instance</returns>
        public static SAOperator? FromWebAPI(Dictionary<string, dynamic> jsonDict, string id)
        {
            //"glomobile":{"name":"GLO Mobile","c":{"19":"\u041d\u0438\u0433\u0435\u0440\u0438\u044f","38":"\u0413\u0430\u043d\u0430"}}
            if (string.IsNullOrEmpty(id) || !jsonDict.ContainsKey("c"))
            {
                return null;
            }
            var res = new List<SAOperator>();
            var name = string.Empty;
            JsonElement el;
            if (jsonDict.ContainsKey("name")) {
                el = jsonDict["name"];
                name = el.GetString() ?? id;
            }
            var countries = new Dictionary<ushort, string?>();
            try
            {
                el = jsonDict["c"];
                var countriesDict = el.Deserialize<Dictionary<string, dynamic>>() ?? new Dictionary<string, dynamic>();
                foreach (var entry in countriesDict)
                {
                    var countryIdStr = entry.Key;
                    if (!ushort.TryParse(countryIdStr, out var countryId))
                    {
                        continue;
                    }
                    countries[countryId] = null;
                    try
                    {
                        el = entry.Value;
                        var countryName = el.GetString();
                        countries[countryId] = countryName;
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Trace.WriteLine(ex);
#endif
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Trace.WriteLine(ex);
#endif
            }
            if (countries.Count == 0)
            {
                return null;
            }
            return new SAOperator(id, name, countries);
        }
    }
}
