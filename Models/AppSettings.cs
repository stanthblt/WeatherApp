using Newtonsoft.Json;

namespace WeatherApp.Models;

public class AppSettings
{
    public string DefaultCity { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public string Theme { get; set; } = "Default";
    public string ApiKey { get; set; } = string.Empty;
} 