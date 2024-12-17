using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SmsActivate.API.Network.Response;
using System.IO;
using System.Diagnostics;

namespace SmsActivate.API.Network
{
    internal partial class Client
    {
        internal const string SADefaultHost = "sms-activate.guru/";//Blocked - sms-activate.org
        internal const string SADefaultApiHost = "api.sms-activate.ae/";//Blocked - api.sms-activate.org

        protected HttpClient ApiClient;
        protected int CountRequest;
        private string _host;
        private string _apiHost;
        private readonly Dictionary<string, IWebClientListener> _webClientListeners;
        private readonly Dictionary<string, IExceptionListener> _exceptionListeners;

        protected string BaseUrl
        {
            get { return "https://" + _host; }
        }

        protected string ApiBaseUrl
        {
            get
            {
                return "https://" + _apiHost;
            }
        }

        protected string ReserveApiBaseUrl
        {
            get
            {
                return "https://api." + _host;
            }
        }

        internal Client()
        {
            _host = SADefaultHost;
            _apiHost = SADefaultApiHost;
            ApiClient = new HttpClient();
            _webClientListeners = new Dictionary<string, IWebClientListener>();
            _exceptionListeners = new Dictionary<string, IExceptionListener>();
        }

        public bool HasWebClientListener(string id)
        {
            return _webClientListeners.ContainsKey(id);
        }

        public bool HasExceptionListener(string id)
        {
            return _exceptionListeners.ContainsKey(id);
        }

        public string AddWebClientListener(IWebClientListener listener)
        {
            var id = Guid.NewGuid().ToString();
            _webClientListeners.Add(id, listener);
            return id;
        }

        public string AddExceptionListener(IExceptionListener listener)
        {
            var id = Guid.NewGuid().ToString();
            _exceptionListeners.Add(id, listener);
            return id;
        }

        public bool RemoveWebClientListener(string id)
        {
            return _webClientListeners.Remove(id);
        }

        public bool RemoveExceptionListener(string id)
        {
            return _exceptionListeners.Remove(id);
        }

        internal bool SetHost(string host)
        {
            if (string.IsNullOrEmpty(host))
            {
                return false;
            }
            var str = host.Replace("https://", "");
            if (!str.EndsWith('/'))
            {
                str += '/';
            }
            _host = str;
            return true;
        }

        internal bool SetApiHost(string apiHost)
        {
            if (string.IsNullOrEmpty(apiHost))
            {
                return false;
            }
            var str = apiHost.Replace("https://", "");
            if (!str.EndsWith('/'))
            {
                str += '/';
            }
            _apiHost = str;
            return true;
        }

        internal HttpRequestMessage BuildRequest(ApiTarget target, Dictionary<string, dynamic>? additionalReqParameters = null, HttpContent? bodyContent = null)
        {
            var baseUrl = BaseUrl;
            if (target.UseApiBaseUrl())
            {
                baseUrl = ApiBaseUrl;
            }
            var urlBuilder = new UriBuilder(baseUrl + target.ReqPath());
            var request = new HttpRequestMessage
            {
                Method = target.Method()
            };
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            request.Headers.UserAgent.Clear();
            //User-Agent mask (Flutter)
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue(productName: "Dart", productVersion: "3.0"));
            if (request.Method == HttpMethod.Get && additionalReqParameters != null && additionalReqParameters.Count > 0)
            {
                foreach (var entry in additionalReqParameters)
                {
                    urlBuilder.Query += '&' + entry.Key + '=' + entry.Value.ToString();
                }
            }
            else if (request.Method == HttpMethod.Post && bodyContent != null)
            {
                request.Content = bodyContent;
            }
            request.RequestUri = urlBuilder.Uri;

            return request;
        }

        internal async Task<GenResponse> RequestAsGenResponse(HttpRequestMessage request)
        {
            var cid = ++CountRequest;
            try
            {
                var response = await ApiClient.SendAsync(request);
                if (response == null)
                {
                    return new GenResponse(statusCode: -1, error: ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty));
                }
                var statusCode = (int)response.StatusCode;                

                var decompressStream = new GZipStream(response.Content.ReadAsStream(), CompressionMode.Decompress);
                byte[] buffer = new byte[4096];
                var answer = "";
                var count = 1;
                do
                {
                    count = decompressStream.Read(buffer, 0, buffer.Length);
                    if (count > 0)
                    {
                        answer += Encoding.UTF8.GetString(buffer, 0, count);
                    }
                } while (count > 0);
                decompressStream.Close();
                decompressStream.Dispose();

                var err = ApiErrorExt.FromResponse(answer);
                if (err == null && (statusCode < 200 || statusCode >= 300))
                {
                    err = ApiErrorEnum.BadResponse;
                }
                if (err != null)
                {
                   
                    var msg = err.Value.Message() + " (" + answer + ')';
                    var apiErr = new ApiError(apiResponse: answer, msg, err);
                    foreach (var listener in _exceptionListeners.Values)
                    {
                        listener.Handle(answer, apiErr);
                    }
                    return new GenResponse(statusCode, answer, apiErr);
                }                
                foreach (var listener in _webClientListeners.Values)
                {
                    listener.Handle(cid, request.RequestUri?.PathAndQuery ?? string.Empty, statusCode, answer);
                }                
                return new GenResponse(statusCode, answer);
            }
            catch (IOException ex)
            {
#if DEBUG
                Trace.WriteLine(ex);
#endif
                var apiErr = ApiErrorEnum.NoConnection.AsException(apiResponse: string.Empty, message: ex.Message);
                foreach (var listener in _exceptionListeners.Values)
                {
                    listener.Handle(errorFromSrv: string.Empty, apiErr);
                }                
                return new GenResponse(statusCode: -1, error: apiErr);
            }
            catch (TimeoutException ex)
            {
#if DEBUG
                Trace.WriteLine(ex);
#endif
                var apiErr = ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty, message: ex.Message);
                foreach (var listener in _exceptionListeners.Values)
                {
                    listener.Handle(errorFromSrv: string.Empty, apiErr);
                }
                return new GenResponse(statusCode: -1, error: apiErr);
            }
            catch (HttpRequestException ex)
            {
#if DEBUG
                Trace.WriteLine(ex);
#endif
                var apiErr = ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty, message: ex.Message);
                foreach (var listener in _exceptionListeners.Values)
                {
                    listener.Handle(errorFromSrv: string.Empty, apiErr);
                }
                var statusCode = -1;
                if (ex.StatusCode != null)
                {
                    statusCode = (int)ex.StatusCode;
                }
                return new GenResponse(statusCode: statusCode, error: apiErr);
            }
            catch (Exception ex)
            {
#if DEBUG
                Trace.WriteLine(ex);
#endif
                var apiErr = ApiErrorEnum.NoResponse.AsException(apiResponse: string.Empty, message: ex.Message);
                foreach (var listener in _exceptionListeners.Values)
                {
                    listener.Handle(errorFromSrv: string.Empty, apiErr);
                }
                return new GenResponse(statusCode: -1, error: apiErr);
            }
        }        
    }
}
