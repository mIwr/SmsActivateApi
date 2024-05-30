namespace SmsActivate.API
{
    /// <summary>
    /// Represents abstract result with positive and (or) negative cases
    /// </summary>
    /// <typeparam name="T">Positive (payload) type</typeparam>
    /// <typeparam name="X">Negative (error) type</typeparam>
    public class Result<T, X>
    {
        /// <summary>
        /// Positive case data
        /// </summary>
        public readonly T? Data;
        /// <summary>
        /// Negative case error data
        /// </summary>
        public readonly X? Error;

        /// <summary>
        /// Returns true if positive case data isn't null and error one is null
        /// </summary>
        public bool Success
        {
            get
            {
                return Data != null && Error == null;
            }
        }

        /// <summary>
        /// Initializes with positive case
        /// </summary>
        /// <param name="data">Positive case data</param>
        public Result(T? data)
        {
            Data = data;
            Error = default;
        }

        /// <summary>
        /// Initializes with negative case
        /// </summary>
        /// <param name="error">Negative case data</param>
        public Result(X? error)
        {
            Data = default;
            Error = error;
        }

        /// <summary>
        /// Initializes with positive and negative cases
        /// </summary>
        /// <param name="data">Positive case data</param>
        /// <param name="error">Negative case data</param>
        public Result(T? data, X? error)
        {
            Data = data;
            Error = error;
        }
    }
}
