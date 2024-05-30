namespace SmsActivate.API.Network.Response
{
    /// <summary>
    /// Low-level API response
    /// </summary>
    internal class GenResponse
    {
        /// <summary>
        /// HTTP status code
        /// </summary>
        internal readonly int StatusCode;
        /// <summary>
        /// Response data
        /// </summary>
        internal readonly string? Data;
        /// <summary>
        /// Parsed API error
        /// </summary>
        internal readonly ApiError? Error;

        internal bool Success
        {
            get
            {
                return StatusCode >= 200 && StatusCode < 300 && Error == null;
            }
        }

        /// <summary>
        /// General response ctor. Includes 'success' and 'fail' data
        /// </summary>
        /// <param name="statusCode">Response status code</param>
        /// <param name="data">Response raw data</param>
        /// <param name="error">Parsed response error</param>
        internal GenResponse(int statusCode, string data, ApiError error)
        {
            StatusCode = statusCode;
            Data = data;
            Error = error;
        }
        /// <summary>
        /// Sucess response ctor
        /// </summary>
        /// <param name="statusCode">Response status code</param>
        /// <param name="data">Response raw data</param>
        internal GenResponse(int statusCode, string? data)
        {
            StatusCode = statusCode;
            Data = data;
            Error = null;
        }
        /// <summary>
        /// Fail response ctor
        /// </summary>
        /// <param name="statusCode">Response status code</param>
        /// <param name="error">Parsed response error</param>
        internal GenResponse(int statusCode, ApiError error)
        {
            StatusCode = statusCode;
            Data = null;
            Error = error;
        }
    }
}
