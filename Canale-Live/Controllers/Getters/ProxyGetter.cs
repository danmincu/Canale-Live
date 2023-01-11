using RestSharp;
using System.Collections.Concurrent;

namespace Canale_Live.Controllers.Getters
{

    public interface IProxyGetter : IDisposable
    {
        string? RefererGetRequest(string uri, out RedirectInfo redirect, int timeout = 3000, int retryCount = 2);
        string? RefererGetRequest(string uri, int timeout = 3000, int retryCount = 2);
        //byte[] GetTs(string uri);
        Task<Stream?> GetTss(string uri, Func<HttpResponseMessage, ValueTask>? afterRequest = null, int timeout=4000);
    }

    public class ProxyGetter : IProxyGetter
    {

        static ProxyGetter? _singleton = null;
        private ILogger<ProxyGetter> _logger;
        private readonly RestClient _client;

        public ProxyGetter(ILogger<ProxyGetter> logger)
        {
            _logger = logger;
            _client = new RestClient(new RestClientOptions() { FollowRedirects = false, MaxTimeout = 6000 }); ;
        }


        public string? RefererGetRequest(string uri, int timeout = 3000, int retryCount = 2)
        {
            return this.RefererGetRequest(uri, out RedirectInfo redirect, timeout, retryCount);
        }

        public string? RefererGetRequest(string uri, out RedirectInfo redirect, int timeout = 3000, int retryCount = 2)
        {
            var attempts = 0;
            redirect = null;
            //var client = new RestClient(new RestClientOptions() { FollowRedirects = false });
            tryagain: var request = new RestRequest(uri, Method.Get);
            this.ApplyHeaders(request, timeout);
            RestResponse response = _client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.MovedPermanently || response.StatusCode == System.Net.HttpStatusCode.Moved ||
                response.StatusCode == System.Net.HttpStatusCode.TemporaryRedirect || response.StatusCode == System.Net.HttpStatusCode.Redirect)
            {
                var newUri = (string)response?.Headers?.Where(h => h.Name == "Location")?.FirstOrDefault()?.Value;
                _logger.LogInformation($"301 Redirect detected:{uri} => {newUri}");
                redirect = new RedirectInfo { FromUrl = new Uri(uri), ToUrl = new Uri(newUri) };
                uri = newUri;
                request = new RestRequest(newUri, Method.Get);
                this.ApplyHeaders(request, timeout);
                response = _client.Execute(request);
            }

            if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                _logger.LogError($"[{response.StatusCode}]:{uri}");
                if (++attempts < retryCount)
                 goto tryagain;
            }
            _logger.LogInformation($"fetting Info stream: {uri}");
            return response?.Content?.ToString();
        }

        public async Task<Stream?> GetTss(string uri, Func<HttpResponseMessage, ValueTask> afterRequest = null, int timeout = 4000)
        {
            _logger.LogInformation($"fetting TS stream: {uri}");
            var request = new RestRequest(uri.Replace(".ts", ".js"), Method.Get);
            this.ApplyHeaders(request, timeout);

            if (afterRequest != null)
            {
                request.OnAfterRequest = afterRequest;
            }

            return await _client!.DownloadStreamAsync(request).ConfigureAwait(false);
        }


        //public byte[] GetTs(string uri)
        //{
        //    var request = new RestRequest(uri, Method.Get);
        //    this.ApplyHeaders(request);
        //    return _client!.DownloadData(request);
        //}

        private void ApplyHeaders(RestRequest request, int timeout = 3000)
        {
            request.Timeout = timeout;
            request.AddHeader("accept", "*/*");
            request.AddHeader("accept-language", "en-US,en;q=0.9,ro;q=0.8");
            request.AddHeader("origin", "https://canale.live");
            request.AddHeader("referer", "https://canale.live/");
            request.AddHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\"");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            request.AddHeader("sec-ch-ua-platform", "\"Windows\"");
            request.AddHeader("sec-fetch-dest", "empty");
            request.AddHeader("sec-fetch-mode", "cors");
            request.AddHeader("sec-fetch-site", "cross-site");
            request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36");
        }

        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
