using System;
using System.Text.Json;

namespace SmsActivate.API.Util
{
    /// <summary>
    /// JSON codec utils
    /// </summary>
    internal static class ModelParseUtil
    {
        internal static bool TryParseUInt16(JsonElement obj, out ushort parsed)
        {
            if (!TryParseUInt64(obj, out var parsedULL))
            {
                parsed = 0;
                return false;
            }
            parsed = (ushort)parsedULL;
            return true;
        }

        internal static bool TryParseUInt32(JsonElement obj, out uint parsed)
        {
            if (!TryParseUInt64(obj, out var parsedULL))
            {
                parsed = 0;
                return false;
            }
            parsed = (uint)parsedULL;
            return true;
        }

        internal static bool TryParseInt32(JsonElement obj, out int parsed)
        {
            if (!TryParseInt64(obj, out var parsedLL))
            {
                parsed = 0;
                return false;
            }
            parsed = (int)parsedLL;
            return true;
        }

        internal static bool TryParseUInt64(JsonElement obj, out ulong parsed)
        {
            if (obj.ValueKind == JsonValueKind.Number)
            {
                if (!obj.TryGetUInt64(out parsed))
                {
                    parsed = 0;
                    return false;
                }
                return true;
            }
            else if (obj.ValueKind == JsonValueKind.String)
            {
                var str = obj.GetString();
                if (string.IsNullOrEmpty(str))
                {
                    parsed = 0;
                    return false;
                }
                if (!ulong.TryParse(str, out parsed))
                {
                    parsed = 0;
                    return false;
                }
                return true;
            }
            parsed = 0;
            return false;
        }

        internal static bool TryParseInt64(JsonElement obj, out long parsed)
        {
            if (obj.ValueKind == JsonValueKind.Number)
            {
                if (!obj.TryGetInt64(out parsed))
                {
                    parsed = 0;
                    return false;
                }
                return true;
            }
            else if (obj.ValueKind == JsonValueKind.String)
            {
                var str = obj.GetString();
                if (string.IsNullOrEmpty(str))
                {
                    parsed = 0;
                    return false;
                }
                if (!long.TryParse(str, out parsed))
                {
                    parsed = 0;
                    return false;
                }
                return true;
            }
            parsed = 0;
            return false;
        }

        internal static bool TryParseDouble(JsonElement obj, out double parsed)
        {
            if (obj.ValueKind == JsonValueKind.Number)
            {
                if (!obj.TryGetDouble(out parsed))
                {
                    if (!obj.TryGetInt64(out var priceLL))
                    {
                        parsed = 0.0;
                        return false;
                    }
                    parsed = Convert.ToDouble(priceLL);
                    return true;
                }
                return true;
            }
            else if (obj.ValueKind == JsonValueKind.String)
            {
                var str = obj.GetString();
                if (string.IsNullOrEmpty(str))
                {
                    parsed = 0.0;
                    return false;
                }
                if (!double.TryParse(str.Replace('.', ','), out parsed))
                {
                    if (!long.TryParse(str, out var priceLL))
                    {
                        parsed = 0.0;
                        return false;
                    }
                    parsed = Convert.ToDouble(priceLL);
                    return true;
                }
                return true;
            }
            parsed = 0.0;
            return false;
        }

        internal static bool TryParseBool(JsonElement obj, out bool parsed)
        {
            if (obj.ValueKind == JsonValueKind.True)
            {
                parsed = true;
                return true;
            }
            if (obj.ValueKind == JsonValueKind.False)
            {
                parsed = false;
                return true;
            }
            if (obj.ValueKind == JsonValueKind.Number)
            {
                if (obj.TryGetByte(out var b) && b <= 1)
                {                    
                    parsed = b == 1;
                    return true;
                }
            }
            else if (obj.ValueKind == JsonValueKind.String)
            {
                var str = obj.GetString();
                if (string.IsNullOrEmpty(str))
                {
                    parsed = false;
                    return false;
                }
                if (byte.TryParse(str, out var b) && b <= 1)
                {
                    parsed = b == 1;
                    return true;
                }
            }
            parsed = false;
            return false;
        }
    }
}
