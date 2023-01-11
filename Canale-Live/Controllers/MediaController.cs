using Canale_Live.Controllers.Getters;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Text;

namespace Canale_Live.Controllers
{    
    public class Redirects: IRedirectCollection
    {
        private ConcurrentDictionary<string, RedirectInfo> _mediaRedirects;
        private ConcurrentDictionary<string, bool> _mediaFlips;
        private ConcurrentDictionary<string, string> _domainsCache;
        
        public Redirects()
        {
            _mediaRedirects = new ConcurrentDictionary<string, RedirectInfo>();
            _mediaFlips = new ConcurrentDictionary<string, bool>();
            _domainsCache = new ConcurrentDictionary<string, string>();
        }
        

        public ConcurrentDictionary<string, RedirectInfo> RedirCollection { get { return _mediaRedirects; } }
        public ConcurrentDictionary<string, bool> MediaFlips { get { return _mediaFlips; } }

        public ConcurrentDictionary<string, string> DomainsCache { get { return _domainsCache; } }
    }

    public interface IRedirectCollection
    {
        ConcurrentDictionary<string, RedirectInfo> RedirCollection { get; }
        ConcurrentDictionary<string, bool> MediaFlips { get; }

        /// <summary>
        /// cache the domains in case it fails to fetch the proper domain
        /// </summary>
        ConcurrentDictionary<string, string> DomainsCache { get; }

    } 

    public class MediaController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProxyGetter _proxy;
        private readonly IConfiguration _configuration;
        private readonly IRedirectCollection _redirectCollection;
        private string[] rotate = new string[] { "1", "3", "4", "5", "6" };
        private Random _random;
        private string _entryPoint;
        private int _getDomainTimeout;
        private int _getDomainRetryCount;
        private readonly int _getTsTimeout;
        private readonly int _getTsRetryCount;

        public MediaController(ILogger<HomeController> logger, IProxyGetter proxy, IConfiguration configuration, IRedirectCollection redirectCollection)
        {
            _logger = logger;
            _proxy = proxy;
            _configuration = configuration;
            _redirectCollection = redirectCollection;
            _random = new Random(DateTime.Now.Millisecond);
            _entryPoint = _configuration.GetValue<string>("EntryPointDomain") ?? "https://cn.webtv1.lol";
            _getDomainTimeout = _configuration.GetValue<int?>("GetDomainTimeout") ?? 1500;
            _getDomainRetryCount = _configuration.GetValue<int?>("GetDomainRetryCount") ?? 2;
            _getTsTimeout = _configuration.GetValue<int?>("GetTsTimeout") ?? 4000;
            _getTsRetryCount = _configuration.GetValue<int?>("GetTsRetryCount") ?? 2;
        }

        public IActionResult? Index4(string a, string b, string c, string? d)
        {
            var context = this.ControllerContext;


            if (d == null && c.Contains("index", StringComparison.InvariantCultureIgnoreCase))
            {
                var index = _proxy.RefererGetRequest($"{_entryPoint}/{a}/{b}/{c}", out RedirectInfo mediaRedirect, 5000, 5);
                if (mediaRedirect != null)
                {
                    mediaRedirect.A = a;
                    mediaRedirect.B = b;
                    mediaRedirect.C = c;

                    try
                    {
                        var uri = mediaRedirect.ToUrl;
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
                _logger.LogDebug(index);
                return Content(index);
            }


            if (c.Contains("tracks", StringComparison.InvariantCultureIgnoreCase) && d.Contains("m3u8", StringComparison.InvariantCultureIgnoreCase))
            {
                // set default
                var newDomain = _configuration.GetValue<string>("EntryPointDomain");
                if (_redirectCollection.RedirCollection.ContainsKey(b))
                {
                    var media_redirects = _redirectCollection.RedirCollection[b];
                    a = media_redirects.ToA;
                    try
                    {
                        var uri = media_redirects.ToUrl;
                        newDomain = $"{uri.Scheme}://{uri.Authority}";
                    }
                    catch
                    {

                    }
                }
                var tracks = _proxy.RefererGetRequest($"{newDomain}/{a}/{b}/{c}/{d}", out RedirectInfo mediaRedirect, 5000, 5);
                _logger.LogDebug(tracks);
                return Content(tracks!);
            }

            return null;
        }

        public async Task<IActionResult> Index9(string a, string b, string c, string d, string e, string f, string g, string h, string i)
        {
            var context = this.ControllerContext;
            var attempts = 0;

            if (c.Contains("tracks", StringComparison.InvariantCultureIgnoreCase) && i.EndsWith("s", StringComparison.InvariantCulture))
            {
                var location = "";
         repeat:if (_redirectCollection.RedirCollection.ContainsKey(b))
                {
                    var media_redirects = _redirectCollection.RedirCollection[b];
                    var redirTrack = $"{media_redirects.ToUrl.Scheme}://{media_redirects.ToUrl.Authority}/{media_redirects.ToA}/{media_redirects.ToB}/{c}/{d}/{e}/{f}/{g}/{h}/{i}";
                    var isRedirect = _proxy.RefererGetRequest(redirTrack, out RedirectInfo trackRedirect, _getDomainTimeout, _getDomainRetryCount);
                    if (trackRedirect != null)
                    {
                        location = trackRedirect.ToUrl.ToString();
                    }

                    a = media_redirects.ToA;
                }

                if (string.IsNullOrEmpty(location))
                {
                    var domainUrl = _configuration.GetValue<string>("MovingTargetDomain");
                    var domain = _proxy.RefererGetRequest(domainUrl!.Replace("%%1%%", rotate[_random.Next(rotate.Length)]), _getDomainTimeout, _getDomainRetryCount);
                    if (!string.IsNullOrEmpty(domain))
                    {
                        this._redirectCollection.DomainsCache.AddOrUpdate(b, domain, (b, nv) => nv = domain);
                    }
                    else
                    {
                        if (_redirectCollection.DomainsCache.TryGetValue(b, out var cachedDomain))
                        {
                            domain = cachedDomain;
                        }
                        else
                            domain = _redirectCollection.DomainsCache.Values.FirstOrDefault();
                    }

                    location = $"https://{domain}/{a}/{b}/{c}/{d}/{e}/{f}/{g}/{h}/{i}".Replace(".ts", ".js");


                    if (_redirectCollection.MediaFlips.TryGetValue(b, out bool val) && val)
                    {
                        location = $"https://{a}.{domain}/{b}/{c}/{d}/{e}/{f}/{g}/{h}/{i}".Replace(".ts", ".js");
                    }
                }

                HttpResponseMessage? code = null;
                Func<HttpResponseMessage, ValueTask> func = (h) => {
                      code = h;
                      return new ValueTask(Task.FromResult<HttpResponseMessage>(h));
                };
               
                var binaryContent = await _proxy.GetTss(location, func, _getTsTimeout).ConfigureAwait(false);

                //bool flipped = false;
                //var location1 = $"https://{a}.{domain}/{b}/{c}/{d}/{e}/{f}/{g}/{h}/{i}".Replace(".ts", ".js");
                //if (code?.StatusCode == System.Net.HttpStatusCode.NotFound && (!_redirectCollection.MediaFlips.TryGetValue(b, out bool val1) || !val1))
                //{
                //    binaryContent = await _proxy.GetTss(location1, func, _getTsTimeout).ConfigureAwait(false);
                //    if (code.StatusCode == System.Net.HttpStatusCode.OK)
                //    {
                //        _redirectCollection.MediaFlips.TryAdd(b, true);
                //        flipped = true;
                //    }
                //    else
                //        _redirectCollection.MediaFlips.TryAdd(b, false);
                //}

                //if (flipped)
                //    _logger.LogInformation($"Flip detected:{location} => {location1}");

                if (code != null &&
                    code.StatusCode != System.Net.HttpStatusCode.NotFound &&
                    code.StatusCode != System.Net.HttpStatusCode.BadRequest &&
                    code.StatusCode != System.Net.HttpStatusCode.OK && 
                    code.StatusCode != System.Net.HttpStatusCode.Accepted)
                {
                    _logger.LogError($"[{code.StatusCode}]:{location}");
                    if (attempts++ < _getTsRetryCount)
                      goto repeat;
                }

                if (binaryContent!= null)
                  return File(binaryContent, "application/octet-stream");
            }

            return NotFound();
        }

    }
}