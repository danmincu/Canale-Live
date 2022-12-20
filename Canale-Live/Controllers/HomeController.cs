using Canale_Live.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Canale_Live.Controllers
{
    public class HomeController : Controller
    {
        Dictionary<string, string> _channels;
        //    = new Dictionary<string, string>()
        //{ {"8", "TVR1" },
        //  {"9", "TVR2" },
        //  {"eantena1", "Antena 1" },
        //  {"antena3", "Antena 3" },
        //  {"protv", "Pro TV" },
        //  {"ptvi", "Pro TV Int" },
        //  {"porno", "Vixen(xxx)" }
        //};

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _channels = config.GetSection("Channels").Get<Dictionary<string, string>>() ?? new Dictionary<string, string>() { { "8", "TVR1" } };
        }

        public IActionResult Index(string? id)
        {
            ViewData["channelIds"] = _channels;// from c in _channels.Keys select c; 
            var key = id ?? "8";
            var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
            ChannelModel channel;
            if (_channels.ContainsKey(key))
              channel = new ChannelModel() { ChannelId = key, ChannelName = _channels[key] };
            else 
              channel = new ChannelModel() { ChannelId = key, ChannelName = key };
            channel.HostUrl = $"{location.Scheme}://{location.Authority}/Media/cdn/{channel.ChannelId}/index.m3u8";
            return View("Index", channel);
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