namespace SmsActivate.API
{
    public partial class SAClient
    {
        /// <summary>
        /// Checks the API response listener existence by ID
        /// </summary>
        /// <param name="id">Listener ID</param>
        /// <returns>True if listener ID is registered, otherwise - false</returns>
        public bool HasWebClientListener(string id)
        {
            return _llClient.HasWebClientListener(id);
        }

        /// <summary>
        /// Checks the exception listener existence by ID
        /// </summary>
        /// <param name="id">Listener ID</param>
        /// <returns>True if listener ID is registered, otherwise - false</returns>
        public bool HasExceptionListener(string id)
        {
            return _llClient.HasExceptionListener(id);
        }

        /// <summary>
        /// Registers API response listener
        /// </summary>
        /// <param name="listener">API response handler</param>
        /// <returns>Listener registration ID</returns>
        public string AddWebClientListener(IWebClientListener listener)
        {
            return _llClient.AddWebClientListener(listener);
        }

        /// <summary>
        /// Registers exception listener
        /// </summary>
        /// <param name="listener">Exception handler</param>
        /// <returns>Listener registration ID</returns>
        public string AddExceptionListener(IExceptionListener listener)
        {
            return _llClient.AddExceptionListener(listener);
        }

        /// <summary>
        /// Removes API response listener by ID
        /// </summary>
        /// <param name="id">Listener ID</param>
        /// <returns>True if listener ID has been removed, otherwise - false</returns>
        public bool RemoveWebClientListener(string id)
        {
            return _llClient.RemoveWebClientListener(id);
        }

        /// <summary>
        /// Removes exception listener by ID
        /// </summary>
        /// <param name="id">Listener ID</param>
        /// <returns>True if listener ID has been removed, otherwise - false</returns>
        public bool RemoveExceptionListener(string id)
        {
            return _llClient.RemoveExceptionListener(id);
        }
    }
}
