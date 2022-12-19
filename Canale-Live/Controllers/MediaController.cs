using Canale_Live.Controllers.Getters;
using Canale_Live.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace Canale_Live.Controllers
{
    public class MediaController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public MediaController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index3(string a,string b, string c, string d, string e, string f)
        {
            var context = this.ControllerContext;

            if (c.Contains("index",StringComparison.InvariantCultureIgnoreCase))
            {
                var index = ProxyGetter.Getm3u8();
               // System.IO.File.WriteAllText($@"C:\tmp\Antena3\index.m3u8", index);
                return Content(index);

            }


            return View();
        }

        public IActionResult Index4(string a, string b, string c, string d)
        {
            var context = this.ControllerContext;

            if (c.Contains("tracks", StringComparison.InvariantCultureIgnoreCase) && d.Contains("m3u8", StringComparison.InvariantCultureIgnoreCase))
            {
                var tracks = ProxyGetter.Getm3u8($"https://webdi.openhd.lol/{a}/{b}/{c}/mono.m3u8");
                //System.IO.File.WriteAllText($@"C:\tmp\Antena3\mono.m3u8", tracks);
                return Content(tracks);
            }

            return View();
        }

        public async Task<FileContentResult> Index9(string a, string b, string c, string d, string e, string f, string g, string h, string i)
        {
            var context = this.ControllerContext;

            if (c.Contains("tracks", StringComparison.InvariantCultureIgnoreCase) && i.EndsWith("s", StringComparison.InvariantCulture))
            {
                var domain = ProxyGetter.GetDomain();
                var location = $"https://{domain}/cdn/antena3/tracks-v1a1/{d}/{e}/{f}/{g}/{h}/{i}";
                var binaryContent = await ProxyGetter.GetTs(domain, location);
                //System.IO.File.WriteAllBytes($@"C:\tmp\Antena3\{i}", binaryContent);
                return File(binaryContent, "application/octet-stream");
            }

            return null;
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}