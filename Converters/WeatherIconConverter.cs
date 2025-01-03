using System;
using System.IO;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System.Net.Http;

namespace WeatherApp.Converters;

public class WeatherIconConverter : IValueConverter
{
    private static readonly HttpClient _httpClient;

    static WeatherIconConverter()
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
        _httpClient = new HttpClient(handler);
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string iconCode && !string.IsNullOrEmpty(iconCode))
        {
            try
            {
                Console.WriteLine($"Converting icon code: {iconCode}");
                var url = $"http://openweathermap.org/img/wn/{iconCode}.png";
                Console.WriteLine($"Icon URL: {url}");
                
                using var response = _httpClient.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    using var stream = response.Content.ReadAsStream();
                    using var memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    return new Bitmap(memoryStream);
                }
                else
                {
                    Console.WriteLine($"HTTP Error: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in converter: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 