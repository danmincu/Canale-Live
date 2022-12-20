using RestSharp;

namespace Canale_Live.Controllers.Getters
{

    public interface IProxyGetter
    {
        string? RefererGetRequest(string uri);
        Task<byte[]?> GetTs(string uri);
    }

    public class ProxyGetter : IProxyGetter
    {
        static ProxyGetter? _singleton = null;

        public static ProxyGetter GetSingleton()
        {
            if (_singleton == null)
            {
                _singleton = new ProxyGetter();
            }
            return _singleton;
        }

        public string? RefererGetRequest(string uri)
        {
            var client = new RestClient();
            var request = new RestRequest(uri, Method.Get);
            this.ApplyHeaders(request);
            RestResponse response = client.Execute(request);
            return response?.Content?.ToString();
        }

        public async Task<byte[]?> GetTs(string uri)
        {
            var client = new RestClient();
            var request = new RestRequest(uri.Replace(".ts", ".js"), Method.Get);
            this.ApplyHeaders(request);
            
            return await client.DownloadDataAsync(request).ConfigureAwait(false);

        }

        private void ApplyHeaders(RestRequest request)
        {
            request.Timeout = -1;
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

    }


}
