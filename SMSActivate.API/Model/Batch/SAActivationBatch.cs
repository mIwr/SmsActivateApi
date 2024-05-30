using System;
using System.Collections.Generic;

namespace SmsActivate.API.Model.Batch
{
    /// <summary>
    /// Activation batch
    /// </summary>
    public class SAActivationBatch: SABatch<SAActivationDetailed>
    {
        public SAActivationBatch(bool existNext, SAActivationDetailed[] data) : base(existNext, data)
        {

        }

        /// <summary>
        /// Try parse batch instance from API response dictionary
        /// </summary>
        /// <param name="jsonDict">API response dictionary</param>
        /// <returns>Optional batch instance</returns>
        public static SAActivationBatch? From(Dictionary<string, dynamic> jsonDict)
        {
            var baseBatch = SABatch<SAActivationDetailed>.From(jsonDict, parser: new Func<Dictionary<string, dynamic>, SAActivationDetailed?>((dict) =>
            {
                return SAActivationDetailed.From(dict);
            }));
            if (baseBatch == null)
            {
                return null;
            }
            return new SAActivationBatch(baseBatch.ExistNextPage, baseBatch.Data.ToArray());
        }
    }
}
