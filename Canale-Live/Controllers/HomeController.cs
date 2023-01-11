using Canale_Live.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Canale_Live.Controllers
{
    public class HomeController : Controller
    {
        Dictionary<string, string> _channels;
        bool _forceHttps;
        bool _forceHttp;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _channels = config.GetSection("Channels").Get<Dictionary<string, string>>() ?? new Dictionary<string, string>() { { "8", "TVR1" } };
            _forceHttps =(bool?)config.GetValue<bool>("ForceHttps") ?? false;
            _forceHttp = (bool?)config.GetValue<bool>("ForceHttp") ?? false;
        }

        public IActionResult Index(string? id)
        {
            ViewData["channelIds"] = _channels;// from c in _channels.Keys select c;             
            var key = id ?? "14";
            ViewData["channelId"] = key;
            var scheme = _forceHttps ? "https" : Request.Scheme;
            if (_forceHttp)
              scheme = "http";
            var location = new Uri($"{scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
            ChannelModel channel;
            if (_channels.ContainsKey(key))
              channel = new ChannelModel() { ChannelId = key, ChannelName = _channels[key] };
            else 
              channel = new ChannelModel() { ChannelId = key, ChannelName = key };
            channel.HostUrl = $"{location.Scheme}://{location.Authority}/Media/lb/{channel.ChannelId}/index.m3u8";
            return View("Index", channel);
        }

    }
}