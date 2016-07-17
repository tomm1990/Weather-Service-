﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace WeatherApp
{
    public class WeatherWorld : WeatherData, IWeatherDataService
    {
        private const String key = "7f41c073c67a4bdbac785947161707";
        //private Location location;
        private static WeatherWorld weatherWorld;
        private WeatherWorld() : base() { }

        public static WeatherWorld Instance()
        {
            if (weatherWorld == null)
            {
                weatherWorld = new WeatherWorld();
            }
            return weatherWorld;
        }

        public override WeatherData getWeatherData(Location location)
        {
            WeatherWorld wd = new WeatherWorld();

            try
            {
                wd.Location.Country = location.Country;
                XMLFunction(wd, location);
            }
            catch (WeatherDataServiceException ex)
            {
                //Console.WriteLine(ex.Message);
            }
            return wd;
        }

        public void XMLFunction(WeatherWorld wd, Location location)
        {
            String URLString = "http://api.worldweatheronline.com/premium/v1/weather.ashx?key=" +
                        key + "&q=" + location.Country + "&num_of_days=1&tp=24&format=xml";
            string xml;
            using (WebClient client = new WebClient())
            {
                try
                {
                    xml = client.DownloadString(URLString);// xml url to string
                }
                catch (WebException)
                {
                    throw new WeatherDataServiceException("There is not internet connection");
                }

            }
            try
            {
                XDocument ob = XDocument.Parse(xml);
                //A linq to xml that get all the values from the site
                var weather = from x in ob.Descendants("data")
                              select new
                              {
                                  City = x.Descendants("query").First().Value,
                                  Ip = x.Descendants("type").First().Value,
                                  Sun = x.Descendants("sunrise").First().Value,
                                  Set = x.Descendants("sunset").First().Value,
                                  Tempat = x.Descendants("temp_C").First().Value,
                                  Cloud = x.Descendants("cloudcover").First().Value,
                                  Humidity = x.Descendants("humidity").First().Value,
                                  Speed = x.Descendants("windspeedKmph").First().Value,
                                  Direction = x.Descendants("winddir16Point").First().Value,
                                  Update = x.Descendants("date").First().Value,
                              };

                //Get all the values from the linq vairables and set 
                //them into the WeatherData service values.
                foreach (var data in weather)
                {
                    //This restful web service also support an ip search.
                    //this check is to confrim that the user pressed a country.
                    if (data.Ip == "IP")
                    {
                        throw new XmlException();
                    }
                    wd.Location.Country = data.City;
                    wd.Location.Sunrise = data.Sun;
                    wd.Location.Sunset = data.Set;
                    wd.Location.Lastupdate = data.Update;
                    wd.Location.Temperature = data.Tempat;
                    wd.Location.Cloud = data.Cloud;
                    wd.Location.Humidity = data.Humidity;
                    wd.Location.Wind = data.Speed;
                }
            }
            catch (XmlException)
            {

                throw new WeatherDataServiceException("Wrong Country");
            }
            catch (WebException)
            {
                throw new WeatherDataServiceException("There is not internet connection");
            }
            catch (InvalidOperationException ex)
            {
                throw new WeatherDataServiceException(ex.Message);
            }
        }


    }
}
