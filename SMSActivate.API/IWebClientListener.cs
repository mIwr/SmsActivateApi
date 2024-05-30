namespace SmsActivate.API
{
    /// <summary>
    /// SMS Activate API response listener interface
    /// </summary>
    public interface IWebClientListener
    {
        /// <summary>
        /// Handle the incoming API response
        /// </summary>
        /// <param name="cid">Request ID</param>
        /// <param name="request">Request Uri</param>
        /// <param name="statusCode">Response status code</param>
        /// <param name="response">Raw response data</param>
        void Handle(int cid, string request, int statusCode, string response);
    }
}
