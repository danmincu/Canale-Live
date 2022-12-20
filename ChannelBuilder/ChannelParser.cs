using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;
using RestSharp;
using System.Collections.Concurrent;

namespace ChannelBuilder
{
    internal class ChannelParser
    {

        public static IDictionary<string, string> Channels()
        {
            var result = new ConcurrentDictionary<string, string>();
            var partials = new List<string>();
            var client = new RestClient();

            var request = new RestRequest("https://canale.live/", Method.Get);
            request.Timeout = -1;
            RestResponse response = client.Execute(request);
            var input = response.Content;


            string pattern = @"<li data-catid=""[a-zA-Z]*""><a style=""border-left-color:#.*?"" href=""(.*?)"">";

            //pattern = @"<a style="border - left - color:#8e140f"  href=(.*?)>";



            RegexOptions options = RegexOptions.Multiline;

            foreach (Match m in Regex.Matches(input, pattern, options))
            {
                //	Console.WriteLine("'{0}' found at index {1}.", m.Value, m.Index);
                if (m.Groups.Count > 1)
                {
                    var href = m.Groups[1].Value;
                    // Console.WriteLine(href);
                    if (!href.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                    {
                        partials.Add(href);
                    }
                }

            }

            //partials.Clear();
            //partials.Add("/tv/6");

            
            Parallel.ForEach(partials, new ParallelOptions() { MaxDegreeOfParallelism = 1}, (item, i) =>
            {
                request = new RestRequest($"https://canale.live{item}", Method.Get);
                request.Timeout = -1;
                response = client.Execute(request);
                input = response.Content;

                string channel_name = "";
                string channel_id = "";
                pattern = @"<title>(.*) \| Vezi programe tv online<\/title>";
                options = RegexOptions.Multiline;
                foreach (Match m in Regex.Matches(input, pattern, options))
                {
                    if (m.Groups.Count > 1)
                    {
                        channel_name = m.Groups[1].Value;
                        break;
                    }
                }

                pattern = @"source: ""https:\/\/zcri.openhd.lol\/lb\/(.*)\/index.m3u8"",";
                foreach (Match m in Regex.Matches(input, pattern, options))
                {
                    if (m.Groups.Count > 1)
                    {
                        channel_id = m.Groups[1].Value;
                        break;
                    }
                }
                Console.WriteLine($"{channel_id}:{channel_name}");
                result.TryAdd(channel_id, channel_name);
            });

            return result;
        }
    }
}


