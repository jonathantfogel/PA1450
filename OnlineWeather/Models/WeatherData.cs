using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace OnlineWeather.Models
{
    public class WeatherData
    {
        public List<WeatherItem> Data { get; internal set; }
        private string DataType { get; }
        public string DataAttribute { get; }

        public WeatherData(DateTime from, DateTime to, int interval, string dataType = "t")
        {
            try
            {
                this.DataAttribute = WeatherAttribute.Attributes[dataType];
                this.DataType = dataType;
            }
            catch
            {
                new KeyNotFoundException($"Weather attribute \"{dataType}\" not found.");
            }
            
            string csvData = File.ReadAllText($"{HttpContext.Current.Server.MapPath("~")}/Content/WeatherData/smhi-{dataType}.csv");

            var test = new List<WeatherItem>();

            TimeSpan timeInterval;
            if (interval == 1)
            {
                timeInterval = new TimeSpan(1, 0, 0, 0);
            }
            else if (interval == 2)
            {
                timeInterval = new TimeSpan(30, 0, 0, 0);
            }
            else
            {
                timeInterval = new TimeSpan(1, 0, 0);
            }
            
            var rows = csvData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            DateTime prevInterval = new DateTime();
            List<double> currentIntervalData = new List<double>();

            var provider = CultureInfo.CreateSpecificCulture("en-GB");

            for (int rowidx = 1; rowidx < rows.Count(); rowidx++)
            {
                if (!String.IsNullOrEmpty(rows[rowidx]))
                {
                    var cells = rows[rowidx].Split(';');
                    var tempDate = DateTime.Parse($"{cells[0]} {cells[1]}");
                    if (tempDate >= from)
                    {
                        if (tempDate.Subtract(prevInterval) < timeInterval)
                        {
                            currentIntervalData.Add(double.Parse(cells[2], provider));
                        }
                        else
                        {
                            currentIntervalData.Add(double.Parse(cells[2], provider));
                            test.Add(new WeatherItem(cells[0], cells[1], ((currentIntervalData.Count > 0) ? currentIntervalData.Average() : 0.0).ToString(provider)));
                            prevInterval = tempDate;
                            currentIntervalData.Clear();
                        }
                    }
                    if (tempDate > to)
                    {
                        break;
                    }
                }
            }
            Data = test;
        }
    }

    public class WeatherItem
    {
        public string Timestamp { get; }
        public string Data { get; }

        public WeatherItem(string date, string time, string data)
        {
            Timestamp = $"{date} {time}";
            Data = data;
        }
    }

    public class WeatherAttribute
    {
        public static Dictionary<string, string> Attributes = new Dictionary<string, string>()
        {
            { "t", "Lufttemperatur" },
            { "msl", "Lufttryck" },
            { "r", "Luftfuktighet" }
        };
    }
}