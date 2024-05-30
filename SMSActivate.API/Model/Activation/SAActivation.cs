using SmsActivate.API.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

namespace SmsActivate.API.Model
{
    /// <summary>
    /// SMS Activate phone number activation
    /// </summary>
    public class SAActivation: IEquatable<SAActivation>
    {
        /// <summary>
        /// Activation ID
        /// </summary>
        public readonly long ID;
        /// <summary>
        /// Activation phone number
        /// </summary>
        public readonly ulong Phone;
        /// <summary>
        /// Activation status
        /// </summary>
        public readonly SAActivationStatus Status;
        /// <summary>
        /// Country ID
        /// </summary>
        public readonly ushort CountryID;
        /// <summary>
        /// Service short name (ID)
        /// </summary>
        public readonly string ServiceID;
        /// <summary>
        /// Can request additional SMS
        /// </summary>
        public readonly bool CanGetAnotherSMS;
        /// <summary>
        /// Activation price
        /// </summary>
        public readonly double Cost;
        /// <summary>
        /// Activation create timestamp (UTC+0)
        /// </summary>
        public readonly long CreateTsUTC;
        /// <summary>
        /// Checks the condition 'The activation can be cancelled after 2 minutes from creation'
        /// </summary>
        public bool CancelBlock
        {
            get
            {
                var nowTsUTC = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var delta = nowTsUTC - CreateTsUTC;
                return delta < 120;
            }
        }
        /// <summary>
        /// Activation create datetime (UTC+0)
        /// </summary>
        public DateTime CreateDateTimeUTC
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(CreateTsUTC).DateTime;
            }
        }
        /// <summary>
        /// Activation create datetime (local)
        /// </summary>
        public DateTime CreateDateTimeLocal
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(CreateTsUTC).LocalDateTime;
            }
        }

        public SAActivation(long id, ulong phone, SAActivationStatus status, ushort countryId, string serviceId, bool canGetAnotherSms, double cost, long createTsUTC)
        {
            ID = id;
            Phone = phone;
            Status = status;
            CountryID = countryId;
            ServiceID = serviceId;
            CanGetAnotherSMS = canGetAnotherSms;
            Cost = cost;
            CreateTsUTC = createTsUTC;
        }

        public override bool Equals(object? obj)
        {
            if (obj is SAActivation == false)
            {
                return false;
            }
            var activation = obj as SAActivation;
            return Equals(activation);            
        }

        public bool Equals(SAActivation? other)
        {
            return other != null && ID == other.ID && Phone == other.Phone;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, Phone);
        }

        public override string ToString()
        {
            return ID.ToString() + " (ServiceID " + ServiceID + ") - " + Phone.ToString();
        }

        /// <summary>
        /// Try parse SMS Activation instance from API response dictionary
        /// </summary>
        /// <param name="jsonDict">API response dictionary</param>
        /// <param name="countryId">Country ID. Optional</param>
        /// <param name="serviceId">Service ID. Optional</param>
        /// <returns>Optional phone activation instance</returns>
        public static SAActivation? From(Dictionary<string, dynamic> jsonDict, ushort? countryId = null, string? serviceId = null)
        {
            if ((!jsonDict.ContainsKey("activationId") && !jsonDict.ContainsKey("id")) || (!jsonDict.ContainsKey("phoneNumber") && !jsonDict.ContainsKey("phone")))
            {
                return null;
            }

            JsonElement obj;
            if (jsonDict.ContainsKey("id"))
            {
                obj = jsonDict["id"];
            }
            else
            {
                obj = jsonDict["activationId"];
            }            
            if (!ModelParseUtil.TryParseInt64(obj, out var id))
            {
                return null;
            }
            if (jsonDict.ContainsKey("phone"))
            {
                obj = jsonDict["phone"];
            }
            else
            {
                obj = jsonDict["phoneNumber"];
            }
            if (!ModelParseUtil.TryParseUInt64(obj, out var phone))
            {
                return null;
            }           
            var status = SAActivationStatus.WAIT_CODE;
            if (jsonDict.ContainsKey("activationStatus") || jsonDict.ContainsKey("status"))
            {
                var str = string.Empty;
                if (jsonDict.ContainsKey("status"))
                {
                    obj = jsonDict["status"];
                    str = obj.GetString();
                }
                else
                {
                    obj = jsonDict["activationStatus"];
                    str = obj.GetString();
                }
                if (!string.IsNullOrEmpty(str) && byte.TryParse(str, out var code))
                {
                    var parsed = SAActivationStatusExt.From(code);
                    if (parsed != null)
                    {
                        status = parsed.Value;
                    }
                }
            }
            var service = serviceId ?? string.Empty;
            if (string.IsNullOrEmpty(serviceId))
            {
                if (jsonDict.ContainsKey("serviceCode"))
                {
                    obj = jsonDict["serviceCode"];
                    service = obj.GetString();
                }
                else if (jsonDict.ContainsKey("service"))
                {
                    obj = jsonDict["service"];
                    service = obj.GetString();
                }
                if (string.IsNullOrEmpty(service))
                {
                    return null;
                }
            }            
            ushort country = 0;
            if (countryId != null)
            {
                country = countryId.Value;
            }
            else if (jsonDict.ContainsKey("countryCode") && jsonDict.ContainsKey("country"))
            {
                obj = jsonDict["countryCode"];
                if (obj.ValueKind == JsonValueKind.Null || !ModelParseUtil.TryParseUInt16(obj, out country))
                {
#if DEBUG
                    Trace.WriteLine("Activation parsing: unable to parse country ID from '" + obj.ToString());
#endif
                }
            }
            else
            {
#if DEBUG
                Trace.WriteLine("Activation parsing: no value for country ID");
#endif
            }
            var canGetAnotherSms = false;
            if (jsonDict.ContainsKey("canGetAnotherSms") || jsonDict.ContainsKey("addSms"))
            {
                if (jsonDict.ContainsKey("canGetAnotherSms"))
                {
                    obj = jsonDict["canGetAnotherSms"];
                }
                else
                {
                    obj = jsonDict["addSms"];
                }                
                if (obj.ValueKind == JsonValueKind.Null || !ModelParseUtil.TryParseBool(obj, out canGetAnotherSms))
                {
#if DEBUG
                    Trace.WriteLine("Activation parsing: unable to parse can get another SMS flag from '" + obj.ToString());
#endif
                }
            }            
            double cost = 0.0;
            if (jsonDict.ContainsKey("activationCost"))
            {
                obj = jsonDict["activationCost"];
                if (obj.ValueKind == JsonValueKind.Null || !ModelParseUtil.TryParseDouble(obj, out cost))
                {
#if DEBUG
                    Trace.WriteLine("Activation parsing: unable to parse cost value from '" + obj.ToString());
#endif
                }
            }
            var createTsUTC = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (jsonDict.ContainsKey("activationTime"))
            {
                obj = jsonDict["activationTime"];
                var str = obj.GetString();//2022-06-01 16:59:16
                if (!string.IsNullOrEmpty(str) && DateTime.TryParseExact(str, format: "yyyy-MM-dd HH:mm:ss", provider: null, style: DateTimeStyles.AssumeLocal, out var dt))
                {
                    var offset = new DateTimeOffset(dt);
                    createTsUTC = offset.ToUniversalTime().ToUnixTimeSeconds();
                }
            }
            else if (jsonDict.ContainsKey("createDate"))
            {
                obj = jsonDict["createDate"];
                if (ModelParseUtil.TryParseInt64(obj, out var parsed))
                {
                    createTsUTC = parsed;
                }
            }            

            return new SAActivation(id, phone, status, country, service, canGetAnotherSms, cost, createTsUTC);           
        }        
    }
}
