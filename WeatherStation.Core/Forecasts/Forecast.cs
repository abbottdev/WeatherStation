using System;
using System.Collections.Generic;
using System.Text;
using WeatherStation.Core.Locations;

namespace WeatherStation.Core.Forecasts
{
    public enum WeatherCodes
    {
        thunderstorm_with_light_rain = 200,
        thunderstorm_with_rain = 201,
        thunderstorm_with_heavy_rain = 202,
        light_thunderstorm = 210,
        thunderstorm = 211,
        heavy_thunderstorm = 212,
        ragged_thunderstorm = 221,
        thunderstorm_with_light_drizzle = 230,
        thunderstorm_with_drizzle = 231,
        thunderstorm_with_heavy_drizzle = 232,
        light_intensity_drizzle = 300,
        drizzle = 301,
        heavy_intensity_drizzle = 302,
        light_intensity_drizzle_rain = 310,
        drizzle_rain = 311,
        heavy_intensity_drizzle_rain = 312,
        shower_rain_and_drizzle = 313,
        heavy_shower_rain_and_drizzle = 314,
        shower_drizzle = 321,
        light_rain = 500,
        moderate_rain = 501,
        heavy_intensity_rain = 502,
        very_heavy_rain = 503,
        extreme_rain = 504,
        freezing_rain = 511,
        light_intensity_shower_rain = 520,
        shower_rain = 521,
        heavy_intensity_shower_rain = 522,
        ragged_shower_rain = 531,
        light_snow = 600,
        snow = 601,
        heavy_snow = 602,
        sleet = 611,
        shower_sleet = 612,
        light_rain_and_snow = 615,
        rain_and_snow = 616,
        light_shower_snow = 620,
        shower_snow = 621,
        heavy_shower_snow = 622,
        mist = 701,
        smoke = 711,
        haze = 721,
        sand = 731,
        fog = 741,
        sand_2 = 751,
        dust = 761,
        volcanic_ash = 762,
        squalls = 771,
        tornado = 781,
        clear_sky = 800,
        few_clouds = 801,
        scattered_clouds = 802,
        broken_clouds = 803,
        overcast_clouds = 804,
        tornado_other = 900,
        hurricane = 902,
        hot = 904,
        hail = 906,
        calm = 951,
        gentle_breeze = 953,
        fresh_breeze = 955,
        high_wind = 957,
        gale = 958,
        storm = 960,
        hurricane_other = 962,
    }


    public class Forecast
    {

        public Locations.Location Location { get; }

        public double Temperature { get; }

        public DateTime ForecastDate { get; }

        public string WeatherDescription { get; }

        public WeatherCodes WeatherCode { get; }

        public string WeatherIconUrl { get; }



        public Forecast(Location location, double temperature, string weatherDescription, string weatherIconUrl, WeatherCodes code, DateTime forecastDate)
        {
            this.WeatherIconUrl = weatherIconUrl;
            this.WeatherCode = code;
            this.WeatherDescription = weatherDescription;
            this.Temperature = temperature;
            this.ForecastDate = forecastDate;
            this.Location = location;
        }

    }
}
