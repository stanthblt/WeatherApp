using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using WeatherApp.Models;

namespace WeatherApp.Services;

public class SettingsService
{
    private const string SettingsFileName = "options.json";
    private readonly string _settingsPath;
    
    public SettingsService()
    {
        // Get the directory where the executable is located
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        
        // Go up two levels from bin/Debug/net9.0 to reach the project root
        string projectDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "..", ".."));
        
        _settingsPath = Path.Combine(projectDirectory, SettingsFileName);
    }

    public async Task<AppSettings> LoadSettingsAsync()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                var defaultSettings = new AppSettings();
                await SaveSettingsAsync(defaultSettings);
                return defaultSettings;
            }

            var json = await File.ReadAllTextAsync(_settingsPath);
            return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
        }
        catch (Exception)
        {
            return new AppSettings();
        }
    }

    public async Task SaveSettingsAsync(AppSettings settings)
    {
        try
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            await File.WriteAllTextAsync(_settingsPath, json);
        }
        catch (Exception)
        {
            // Log error or handle appropriately
        }
    }
} 