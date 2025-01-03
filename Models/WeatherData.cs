using System;
using Newtonsoft.Json;

namespace WeatherApp.Models;

public class WeatherData
{
    public string CityName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Temperature { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Humidity { get; set; }
    public string WeatherIcon { get; set; } = string.Empty;
    public HourlyForecast[] HourlyForecasts { get; set; } = Array.Empty<HourlyForecast>();
    public DailyForecast[] DailyForecasts { get; set; } = Array.Empty<DailyForecast>();
}

public class HourlyForecast
{
    public DateTime DateTime { get; set; }
    public double Temperature { get; set; }
    public int Humidity { get; set; }
    public string WeatherIcon { get; set; } = string.Empty;
}

public class DailyForecast
{
    public DateTime Date { get; set; }
    public double Temperature { get; set; }
    public int Humidity { get; set; }
    public string WeatherIcon { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public HourlyForecast[] HourlyForecasts { get; set; } = Array.Empty<HourlyForecast>();
} 