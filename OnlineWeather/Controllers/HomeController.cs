using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml;
using OnlineWeather.Models;

namespace OnlineWeather.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RequestWeatherData(string attr_t, string attr_msl, string attr_r, int interval, int start_interval_day, int start_interval_month, int end_interval_day, int end_interval_month)
        {
            DateTime dateFrom;
            DateTime dateTo;

            try { dateFrom = new DateTime(2020, start_interval_month, start_interval_day); }
            catch { dateFrom = new DateTime(2020, 1, 5); }

            try { dateTo = new DateTime(2020, end_interval_month, end_interval_day); }
            catch { dateTo = new DateTime(2020, 5, 14); }

            var result = new List<Pair>();

            var attributes = new List<string>();
            if (attr_t == "on")
                attributes.Add("t");
            if (attr_msl == "on")
                attributes.Add("msl");
            if (attr_r == "on")
                attributes.Add("r");

            foreach (var attr in attributes)
            {
                var wd = new WeatherData(dateFrom, dateTo, interval, attr);
                var dataset = new Pair(wd.DataAttribute, wd.Data);
                result.Add(dataset);
            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult RequestYrData()
        {
            XmlDocument doc = new XmlDocument();
            //doc.Load($"{Server.MapPath("~")}/Content/WeatherData/forecast_hour_by_hour.xml"); //Lokal version
            doc.Load("https://www.yr.no/place/Sweden/Blekinge/Karlskrona/forecast_hour_by_hour.xml");

            var updateTime = doc.GetElementsByTagName("lastupdate")[0].InnerText.Replace('T', ' ');

            var forecast = new List<Dictionary<string, string>>();
            var xmlTime = doc.GetElementsByTagName("time");

            for (int i = 0; i < 13; i++)
            {
                forecast.Add(new Dictionary<string, string>(){
                    { "temp", xmlTime[i].SelectSingleNode("temperature").Attributes["value"].Value },
                    { "timestamp", xmlTime[i].Attributes["from"].Value },
                    { "description", xmlTime[i].SelectSingleNode("symbol").Attributes["name"].Value },
                    { "windDirection", xmlTime[i].SelectSingleNode("windDirection").Attributes["deg"].Value },
                    { "windSpeed", xmlTime[i].SelectSingleNode("windSpeed").Attributes["mps"].Value },
                    { "precipitation", xmlTime[i].SelectSingleNode("precipitation").Attributes["value"].Value },
                    { "airPressure", xmlTime[i].SelectSingleNode("pressure").Attributes["value"].Value }
                });

            }
            
            return Json(new Pair(updateTime, forecast));
        }
    }
}