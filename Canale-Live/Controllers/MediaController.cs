using Canale_Live.Controllers.Getters;
using Microsoft.AspNetCore.Mvc;

namespace Canale_Live.Controllers
{
    public class MediaController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProxyGetter _proxy;
        private readonly IConfiguration _configuration;

        public MediaController(ILogger<HomeController> logger, IProxyGetter proxy, IConfiguration configuration)
        {
            _logger = logger;
            _proxy = proxy;
            _configuration = configuration;
        }

        public IActionResult Index4(string a, string b, string c, string? d)
        {
            var context = this.ControllerContext;


            if (d == null && c.Contains("index", StringComparison.InvariantCultureIgnoreCase))
            {
                var index = ProxyGetter.GetSingleton().RefererGetRequest($"https://webdi.openhd.lol/{a}/{b}/{c}");
                return Content(index);
            }


            if (c.Contains("tracks", StringComparison.InvariantCultureIgnoreCase) && d.Contains("m3u8", StringComparison.InvariantCultureIgnoreCase))
            {
                var tracks = ProxyGetter.GetSingleton().RefererGetRequest($"https://webdi.openhd.lol/{a}/{b}/{c}/{d}");
                return Content(tracks);
            }

            return null;
        }

        public async Task<FileContentResult> Index9(string a, string b, string c, string d, string e, string f, string g, string h, string i)
        {
            var context = this.ControllerContext;

            if (c.Contains("tracks", StringComparison.InvariantCultureIgnoreCase) && i.EndsWith("s", StringComparison.InvariantCulture))
            {
                var domainUrl = _configuration.GetValue<string>("MovingTargetDomain");
                var domain = _proxy.RefererGetRequest(domainUrl);
                var location = $"https://{domain}/{a}/{b}/{c}/{d}/{e}/{f}/{g}/{h}/{i}";
                var binaryContent = await _proxy.GetTs(location);
                //System.IO.File.WriteAllBytes($@"C:\tmp\{i}", binaryContent);
                return File(binaryContent, "application/octet-stream");
            }

            return null;
        }

    }
}