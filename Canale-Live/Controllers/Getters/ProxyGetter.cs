using RestSharp;
using System.Text;

namespace Canale_Live.Controllers.Getters
{
    public class ProxyGetter
    {
        public static string GetDomain()
        {
            var client = new RestClient();
            var request = new RestRequest("https://no1.openhd.lol/domain.txt", Method.Get);
            request.Timeout = -1;
            request.AddHeader("authority", "no1.openhd.lol");
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
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            return response.Content;
        }

        public static string Getm3u8(string uri="https://webdi.openhd.lol/cdn/antena3/index.m3u8")
        {
            var client = new RestClient();
            var request = new RestRequest(uri, Method.Get);
            request.Timeout = -1;
            request.AddHeader("authority", "webdi.openhd.lol");
            request.AddHeader("accept", "*/*");
            request.AddHeader("accept-language", "en-US,en;q=0.9,ro;q=0.8");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("origin", "null");
            request.AddHeader("pragma", "no-cache");
            request.AddHeader("referer", "https://canale.live/");
            request.AddHeader("sec-ch-ua", "\"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\"");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            request.AddHeader("sec-ch-ua-platform", "\"Windows\"");
            request.AddHeader("sec-fetch-dest", "empty");
            request.AddHeader("sec-fetch-mode", "cors");
            request.AddHeader("sec-fetch-site", "cross-site");
            request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36");
            RestResponse response = client.Execute(request);            
            return response.Content.ToString();
        }

        public async static Task<byte[]> GetTs(string domain, string uri)
        {
            var client = new RestClient();
            var request = new RestRequest(uri.Replace(".ts",".js"), Method.Get);
            request.Timeout = -1;
            request.AddHeader("authority", domain);
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
            return await client.DownloadDataAsync(request).ConfigureAwait(false);
            //RestResponse response = client.Execute(request);
            //var size = response.ContentLength;
            //return Encoding.ASCII.GetBytes(response.Content ?? "");
            //byte[] bytes = response.Content;
            //Console.WriteLine(response.Content);
            //return response.ToString();
        }

    }


}
