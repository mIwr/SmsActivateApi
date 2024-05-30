using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace SmsActivate.API.Model
{
    /// <summary>
    /// SMS Activate phone activation with SMS info
    /// </summary>
    public class SAActivationDetailed: SAActivation, IEquatable<SAActivationDetailed>
    {
        /// <summary>
        /// Parsed SMS codes
        /// </summary>
        public readonly string[] SmsCodes;
        /// <summary>
        /// Stock received SMS messages
        /// </summary>
        public readonly string[] SmsFullTexts;
        /// <summary>
        /// The recent parsed SMS code
        /// </summary>
        public string SmsCode
        {
            get
            {
                if (SmsCodes.Length == 0)
                {
                    return string.Empty;
                }
                return SmsCodes[0];
            }
        }
        /// <summary>
        /// The recent received stock SMS message
        /// </summary>
        public string SmsFullText
        {
            get
            {
                if (SmsFullTexts.Length == 0)
                {
                    return string.Empty;
                }
                return SmsFullTexts[0];
            }
        }

        public SAActivationDetailed(long id, ulong phone, SAActivationStatus status, ushort countryId, string serviceId, bool canGetAnotherSms, double cost, long createTsUTC, string[] smsCodes, string[] smsFullTexts): base(id, phone, status, countryId, serviceId, canGetAnotherSms, cost, createTsUTC)
        {
            SmsCodes = smsCodes;
            SmsFullTexts = smsFullTexts;
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(SAActivationDetailed? other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Try parse SMS Activation instance from API response dictionary
        /// </summary>
        /// <param name="jsonDict">API response dictionary</param>
        /// <param name="countryId">Country ID. Optional</param>
        /// <param name="serviceId">Service ID. Optional</param>
        /// <returns>Optional phone activation instance</returns>
        public static new SAActivationDetailed? From(Dictionary<string, dynamic> jsonDict, ushort? countryId = null, string? serviceId = null)
        {
            var baseActivation = SAActivation.From(jsonDict, countryId, serviceId);
            if (baseActivation == null)
            {
                return null;
            }
            JsonElement obj;
            var smsCodes = Array.Empty<string>();
            if (jsonDict.ContainsKey("smsCode") || jsonDict.ContainsKey("code"))
            {
                try
                {
                    if (jsonDict.ContainsKey("smsCode"))
                    {
                        obj = jsonDict["smsCode"];
                        if (obj.ValueKind == JsonValueKind.String)
                        {
                            var str = obj.GetString();
                            if (!string.IsNullOrEmpty(str))
                            {
                                smsCodes = new string[] { str };
                            }                            
                        }
                        else if (obj.ValueKind == JsonValueKind.Array)
                        {
                            smsCodes = obj.Deserialize<string[]>() ?? Array.Empty<string>();
                        }
                    }
                    else
                    {
                        obj = jsonDict["code"];
                        var str = obj.GetString();
                        if (!string.IsNullOrEmpty(str))
                        {
                            smsCodes = new string[] { str };
                        }                        
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Trace.WriteLine("SMS code is null");
                    Trace.WriteLine(ex);
#endif
                }
            }
            var smsFullTexts = Array.Empty<string>();
            if (jsonDict.ContainsKey("smsText"))
            {
                try
                {
                    obj = jsonDict["smsText"];
                    if (obj.ValueKind == JsonValueKind.String)
                    {
                        var singleMsg = obj.GetString();
                        if (!string.IsNullOrEmpty(singleMsg))
                        {
                            smsFullTexts = new string[] { singleMsg };
                        }
                        if (jsonDict.ContainsKey("moreSms"))
                        {
                            try
                            {
                                JsonElement el = jsonDict["moreSms"];
                                var arr = el.Deserialize<string[]>() ?? Array.Empty<string>();
                                var combinedSms = new List<string>();
                                var foundSingle = false;
                                foreach (var item in arr)
                                {
                                    if (!string.IsNullOrEmpty(singleMsg) && item == singleMsg)
                                    {
                                        foundSingle = true;
                                    }
                                    combinedSms.Add(item);
                                }
                                if (combinedSms.Count > 0)
                                {
                                    if (!foundSingle && !string.IsNullOrEmpty(singleMsg))
                                    {
                                        combinedSms.Insert(index: 0, singleMsg);
                                    }
                                    smsFullTexts = combinedSms.ToArray();
                                }
                            } 
                            catch
                            {

                            }
                        }
                    }
                    else if (obj.ValueKind == JsonValueKind.Array)
                    {
                        smsFullTexts = obj.Deserialize<string[]>() ?? Array.Empty<string>();
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Trace.WriteLine("SMS full text is null");
                    Trace.WriteLine(ex);
#endif
                }
            }

            return new SAActivationDetailed(baseActivation.ID, baseActivation.Phone, baseActivation.Status, baseActivation.CountryID, baseActivation.ServiceID, baseActivation.CanGetAnotherSMS, baseActivation.Cost, baseActivation.CreateTsUTC, smsCodes, smsFullTexts);
        }        
    }
}