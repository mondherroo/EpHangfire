using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EpHangFireApp.Models;
using RestSharp;
using Microsoft.Extensions.Caching.Memory;

namespace EpHangFireApp.Controllers
{
    public class HomeController : Controller
    {
        private IMemoryCache _memoryCache;
        public HomeController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public IActionResult Index()
        {
            if (_memoryCache.TryGetValue("api", out string cont)) return Ok(cont);
            var client = new RestClient("ht://185.132.133.93:3451/kl91tf480bb9793ny211993dzw71556");
            var request = new RestRequest(Method.GET);
            request.AddHeader("api-rbs-key", "G5G9NMLB501159595494509051");
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful == false) return NotFound();
            cont = response.Content;
            _memoryCache.Set("api", cont, new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(24)));
            return Ok(cont);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
