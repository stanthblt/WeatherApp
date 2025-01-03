using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WeatherApp.Models;

namespace WeatherApp.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    
    public WeatherService(string apiKey)
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
        _httpClient = new HttpClient(handler);
        _apiKey = apiKey;
    }

    public async Task<(double Lat, double Lon)?> GetCityCoordinatesAsync(string cityName)
    {
        try
        {
            var response = await _httpClient.GetStringAsync(
                $"https://api.openweathermap.org/geo/1.0/direct?q={cityName}&limit=1&appid={_apiKey}");
            var results = JsonConvert.DeserializeObject<GeocodingResult[]>(response);
            
            if (results == null || results.Length == 0)
                return null;

            return (results[0].Lat, results[0].Lon);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<WeatherData?> GetWeatherDataAsync(double lat, double lon, string lang = "en")
    {
        try
        {
            var response = await _httpClient.GetStringAsync(
                $"https://api.openweathermap.org/data/3.0/onecall?lat={lat}&lon={lon}&lang={lang}&units=metric&appid={_apiKey}");
            
            System.Diagnostics.Debug.WriteLine("Raw API Response:");
            System.Diagnostics.Debug.WriteLine(response);
            
            var data = JsonConvert.DeserializeObject<OpenWeatherResponse>(response);
            
            if (data == null) return null;
            
            // Debug output for weather icon
            System.Diagnostics.Debug.WriteLine($"Current weather icon from API: {data.Current.Weather[0].Icon}");
            
            // Get city name from reverse geocoding
            var geoResponse = await _httpClient.GetStringAsync(
                $"https://api.openweathermap.org/geo/1.0/reverse?lat={lat}&lon={lon}&limit=1&appid={_apiKey}");
            var geoData = JsonConvert.DeserializeObject<GeocodingResult[]>(geoResponse);
            var cityName = geoData?.FirstOrDefault()?.Name ?? "Unknown Location";

            var weatherData = new WeatherData
            {
                CityName = cityName,
                Latitude = lat,
                Longitude = lon,
                Temperature = data.Current.Temp,
                Description = data.Current.Weather[0].Description,
                Humidity = data.Current.Humidity,
                WeatherIcon = data.Current.Weather[0].Icon, // Make sure this is being set
                
                HourlyForecasts = data.Hourly
                    .Take(6)
                    .Select(h => new HourlyForecast
                    {
                        DateTime = DateTimeOffset.FromUnixTimeSeconds(h.Dt).DateTime,
                        Temperature = h.Temp,
                        Humidity = h.Humidity,
                        WeatherIcon = h.Weather[0].Icon // Make sure this is being set
                    })
                    .ToArray(),
                
                DailyForecasts = data.Daily
                    .Skip(1)
                    .Take(5)
                    .Select(d => new DailyForecast
                    {
                        Date = DateTimeOffset.FromUnixTimeSeconds(d.Dt).DateTime.Date.AddHours(12),
                        Temperature = d.Temp.Day,
                        Humidity = d.Humidity,
                        WeatherIcon = d.Weather[0].Icon, // Make sure this is being set
                        Description = d.Weather[0].Description
                    })
                    .ToArray()
            };

            // Debug output
            System.Diagnostics.Debug.WriteLine($"Current Weather Icon: {weatherData.WeatherIcon}");
            foreach (var forecast in weatherData.DailyForecasts)
            {
                System.Diagnostics.Debug.WriteLine($"Daily Forecast Icon: {forecast.WeatherIcon}");
            }

            return weatherData;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting weather data: {ex.Message}");
            return null;
        }
    }

    public async Task<Stream?> GetWeatherIconAsync(string iconCode)
    {
        try
        {
            var url = $"https://openweathermap.org/img/wn/{iconCode}@2x.png";
            Console.WriteLine($"Fetching icon from: {url}");
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Icon fetched successfully.");
                return await response.Content.ReadAsStreamAsync();
            }
            else
            {
                Console.WriteLine($"Failed to fetch icon. HTTP Status: {response.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading icon: {ex.Message}");
            return null;
        }
    }

    private class OpenWeatherResponse
    {
        public CurrentWeather Current { get; set; } = new();
        public HourlyWeather[] Hourly { get; set; } = Array.Empty<HourlyWeather>();
        public DailyWeather[] Daily { get; set; } = Array.Empty<DailyWeather>();
    }

    private class CurrentWeather
    {
        public long Dt { get; set; }
        public double Temp { get; set; }
        public int Humidity { get; set; }
        public Weather[] Weather { get; set; } = Array.Empty<Weather>();
    }

    private class HourlyWeather
    {
        public long Dt { get; set; }
        public double Temp { get; set; }
        public int Humidity { get; set; }
        public Weather[] Weather { get; set; } = Array.Empty<Weather>();
    }

    private class DailyWeather
    {
        public long Dt { get; set; }
        public TemperatureInfo Temp { get; set; } = new();
        public int Humidity { get; set; }
        public Weather[] Weather { get; set; } = Array.Empty<Weather>();
    }

    private class TemperatureInfo
    {
        public double Day { get; set; }
    }

    private class Weather
    {
        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;
        
        [JsonProperty("icon")]
        public string Icon { get; set; } = string.Empty;
    }
}

public class GeocodingResult
{
    public string Name { get; set; } = string.Empty;
    public double Lat { get; set; }
    public double Lon { get; set; }
} 