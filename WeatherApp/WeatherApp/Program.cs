﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp{
    class Program{
        static void Main(string[] args){
            IWeatherDataService service = WeatherDataServiceFactory.getWeatherDataService(
                WeatherDataServiceFactory.WORLD_WEATHER_ONLINE);
            WeatherData wd = service.getWeatherData(  new Location("Paris")   );
            wd.Start();
            Console.ReadKey();
        }
    }
}
