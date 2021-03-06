﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EpHangFireApp.Models;
using RestSharp;
using Microsoft.Extensions.Caching.Memory;
using System.Xml.Linq;
using System.Globalization;
using System.Net;

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
            var client = new RestClient("http://localhost:51372/api/timezone");
            var request = new RestRequest(Method.GET);
            IPHostEntry heserver = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = heserver.AddressList[2].ToString();
            if (ipAddress == null) return NotFound();
            request.AddParameter("ip", ipAddress);
            IRestResponse response = client.Execute(request);
            string[] timestr = response.Content.Split('\"');
            var offset = CalculateOffset(timestr[1]);
            if (_memoryCache.TryGetValue("api", out List<EpgProgram> cont)) return Ok(cont);
            XDocument xDoc = XDocument.Load("http://epgguide.net?epg=249&auth=67d2f94307e710cc38b1656b08df25&.xml");
            List<EpgProgram> EpList = xDoc.Descendants("programme").Where(e => e.Element("desc") != null).Select
                (ep => new EpgProgram
                {
                    ChannelId = ep.Attribute("channel").Value,
                    Start = DateTime.ParseExact(ep.Attribute("start").Value, "yyyyMMddHHmmss zzzzz", CultureInfo.InvariantCulture).AddHours(offset),
                    Finish = DateTime.ParseExact(ep.Attribute("stop").Value, "yyyyMMddHHmmss zzzzz", CultureInfo.InvariantCulture).AddHours(offset),
                    Title = ep.Element("title").Value,
                    Desc = ep.Element("desc").Value
                }).ToList();
            if (EpList == null) return NotFound();
            cont = EpList;
            _memoryCache.Set("api", cont, new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(24)));
            return Ok(cont);
        }

        public int CalculateOffset(string offset)
        {
            if (string.IsNullOrWhiteSpace(offset)) return 0;
            try
            {
                ;
                //string z = offset.Replace(":", ".");
                String z1 = offset.Substring(0, 3);
                Int32.TryParse(z1, out int j);
                return j;
            }
            catch
            {
                return 0;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
