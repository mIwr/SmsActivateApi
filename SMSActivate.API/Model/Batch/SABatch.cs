using SmsActivate.API.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace SmsActivate.API.Model.Batch
{
    /// <summary>
    /// General-purpose paged batch
    /// </summary>
    /// <typeparam name="T">Batch content type</typeparam>
    public class SABatch<T> where T : IEquatable<T>
    {
        //public uint RemoteTotal { get; private set; }
        /// <summary>
        /// The current page isn't the last one flag
        /// </summary>
        public bool ExistNextPage { get; private set; }
        /// <summary>
        /// Batch content
        /// </summary>
        public List<T> Data { get; private set; }

        public SABatch(/*uint remoteTotal, */bool existNext, T[] data) {
            //RemoteTotal = remoteTotal;
            ExistNextPage = existNext;
            Data = new List<T>(data);
        }

        /// <summary>
        /// Try parse batch instance from API response dictionary
        /// </summary>
        /// <param name="jsonDict">API response dictionary</param>
        /// <param name="parser">Batch content parser</param>
        /// <returns>Optional batch instance</returns>
        public static SABatch<T>? From(Dictionary<string, dynamic> jsonDict, Func<Dictionary<string, dynamic>, T?> parser)
        {
            if (!jsonDict.ContainsKey("data"))
            {
                return null;
            }            
            var dataArr = Array.Empty<Dictionary<string, dynamic>>();
            try
            {
                JsonElement obj = jsonDict["data"];
                dataArr = obj.Deserialize<Dictionary<string, dynamic>[]>() ?? dataArr;
            }
            catch (Exception ex)
            {
#if DEBUG
                Trace.WriteLine("Batch data parse fail");
                Trace.WriteLine(ex);
#endif
            }
            var parsed = new List<T>();
            foreach (var dataDict in dataArr)
            {
                var item = parser(dataDict);
                if (item == null)
                {
                    continue;
                }
                parsed.Add(item);
            }
            /*uint totalRemote = (uint)dataArr.Length;
            if (jsonDict.ContainsKey("quant") && jsonDict["quant"] != null)
            {
                JsonElement obj = jsonDict["quant"];
                if (ModelParseUtil.TryParseUInt32(obj, out var count))
                {
                    totalRemote = count;
                }
            }*/
            var existNext = false;
            if (jsonDict.ContainsKey("existNext"))
            {
                JsonElement obj = jsonDict["existNext"];
                _ = ModelParseUtil.TryParseBool(obj, out existNext);
            }

            return new SABatch<T>(existNext, parsed.ToArray());
        }
    }
}
