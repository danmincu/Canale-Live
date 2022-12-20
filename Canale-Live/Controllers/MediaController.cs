﻿using Canale_Live.Controllers.Getters;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Text;

namespace Canale_Live.Controllers
{


    public class Redirects: IRedirectCollection
    {
        private ConcurrentDictionary<string, RedirectInfo> _mediaRedirects;
        private ConcurrentDictionary<string, bool> _mediaFlips;
        public Redirects()
        {
            _mediaRedirects = new ConcurrentDictionary<string, RedirectInfo>();
            _mediaFlips = new ConcurrentDictionary<string, bool>();
        }
        

        public ConcurrentDictionary<string, RedirectInfo> RedirCollection { get { return _mediaRedirects; } }
        public ConcurrentDictionary<string, bool> MediaFlips { get { return _mediaFlips; } }
    }

    public interface IRedirectCollection
    {
        ConcurrentDictionary<string, RedirectInfo> RedirCollection { get; }
        ConcurrentDictionary<string, bool> MediaFlips { get; }
    } 

    public class MediaController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProxyGetter _proxy;
        private readonly IConfiguration _configuration;
        private readonly IRedirectCollection _redirectCollection;

        public MediaController(ILogger<HomeController> logger, IProxyGetter proxy, IConfiguration configuration, IRedirectCollection redirectCollection)
        {
            _logger = logger;
            _proxy = proxy;
            _configuration = configuration;
            _redirectCollection = redirectCollection;            
        }

        public IActionResult? Index4(string a, string b, string c, string? d)
        {
            var context = this.ControllerContext;


            if (d == null && c.Contains("index", StringComparison.InvariantCultureIgnoreCase))
            {

                //https://zcri.openhd.lol/lb/8/index.m3u8

                //var index = ProxyGetter.GetSingleton().RefererGetRequest($"https://webdi.openhd.lol/{a}/{b}/{c}");

                var index = ProxyGetter.GetSingleton().RefererGetRequest($"https://zcri.openhd.lol/{a}/{b}/{c}", out RedirectInfo mediaRedirect);
                if (mediaRedirect != null)
                {
                    mediaRedirect.A = a;
                    mediaRedirect.B = b;
                    mediaRedirect.C = c;

                    try
                    {
                        var uri = new Uri(mediaRedirect.ToUrl);
                        var localPath = uri.LocalPath;
                        var split = localPath.Trim('/').Split('/');
                        if (split.Length > 0)
                         mediaRedirect.ToA = split[0];
                        if (split.Length > 1)
                          mediaRedirect.ToB = split[1];
                        if (split.Length > 2)
                          mediaRedirect.ToC = split[2];
                    }
                    catch
                    {

                    }
                  
                    _redirectCollection.RedirCollection.AddOrUpdate(b, mediaRedirect, (k,v) => mediaRedirect);
                }
                return Content(index);
            }


            if (c.Contains("tracks", StringComparison.InvariantCultureIgnoreCase) && d.Contains("m3u8", StringComparison.InvariantCultureIgnoreCase))
            {
                if (_redirectCollection.RedirCollection.ContainsKey(b))
                {
                    var media_redirects = _redirectCollection.RedirCollection[b];
                    a = media_redirects.ToA;
                }
                var tracks = ProxyGetter.GetSingleton().RefererGetRequest($"https://webdi.openhd.lol/{a}/{b}/{c}/{d}", out RedirectInfo mediaRedirect);
                //var replacedTracks = ReplaceTracks(tracks);
                return Content(tracks!);
                //return Content(replacedTracks);
            }

            return null;
        }


        //private string ReplaceTracks(string tracks)
        //{
        //    var domainUrl = _configuration.GetValue<string>("MovingTargetDomain");
        //    var domain = _proxy.RefererGetRequest(domainUrl);

        //    var lines = tracks.Split("\n");
        //    var sb = new StringBuilder();
        //    foreach (var item in lines)
        //    {
        //        if (!item.StartsWith("#"))
        //        {
        //            var newLocation = $"https://{domain}/cdn/8/tracks-v1a1/{item}";
        //            newLocation = newLocation.Replace(".ts", ".js");
        //            sb.Append(newLocation);
        //        }
        //        else
        //            sb.AppendLine(item);
        //    }
        //    return sb.ToString();
        //}


        public async Task<FileStreamResult?> Index9(string a, string b, string c, string d, string e, string f, string g, string h, string i)
        {
            var context = this.ControllerContext;

            if (c.Contains("tracks", StringComparison.InvariantCultureIgnoreCase) && i.EndsWith("s", StringComparison.InvariantCulture))
            {
                if (_redirectCollection.RedirCollection.ContainsKey(b))
                {
                    var media_redirects = _redirectCollection.RedirCollection[b];
                    a = media_redirects.ToA;
                }

                var domainUrl = _configuration.GetValue<string>("MovingTargetDomain");
                var domain = _proxy.RefererGetRequest(domainUrl);


                

                var location = $"https://{domain}/{a}/{b}/{c}/{d}/{e}/{f}/{g}/{h}/{i}".Replace(".ts", ".js");

                if (_redirectCollection.MediaFlips.TryGetValue(b, out bool val) && val)
                {
                    location = $"https://{a}.{domain}/{b}/{c}/{d}/{e}/{f}/{g}/{h}/{i}".Replace(".ts", ".js");
                }

                HttpResponseMessage? code = null;
                Func<HttpResponseMessage, ValueTask> func = (h) => {
                      code = h;
                      return new ValueTask(Task.FromResult<HttpResponseMessage>(h));
                };
               
                var binaryContent = await _proxy.GetTss(location, func).ConfigureAwait(false);

                if (code?.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    location = $"https://{a}.{domain}/{b}/{c}/{d}/{e}/{f}/{g}/{h}/{i}".Replace(".ts", ".js");
                    binaryContent = await _proxy.GetTss(location, func).ConfigureAwait(false);
                    if (code.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        _redirectCollection.MediaFlips.TryAdd(b, true);                        
                    }
                    else
                        _redirectCollection.MediaFlips.TryAdd(b, false);
                }


                if (binaryContent!= null)
                  return File(binaryContent, "application/octet-stream");
            }

            return null;
        }


        public FileContentResult Index_not_efficient(string a, string b, string c, string d, string e, string f, string g, string h, string i)
        {
            var context = this.ControllerContext;

            if (c.Contains("tracks", StringComparison.InvariantCultureIgnoreCase) && i.EndsWith("s", StringComparison.InvariantCulture))
            {
                var domainUrl = _configuration.GetValue<string>("MovingTargetDomain");
                var domain = _proxy.RefererGetRequest(domainUrl);
                var location = $"https://{domain}/{a}/{b}/{c}/{d}/{e}/{f}/{g}/{h}/{i}".Replace(".ts", ".js");
                var binaryContent = _proxy.GetTs(location);

                if (binaryContent != null)
                    return File(binaryContent, "application/octet-stream");
            }

            return null;
        }

    }
}