using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Layout;
using WeatherApp.Services;
using WeatherApp.Models;
using Avalonia;
using Avalonia.Styling;

namespace WeatherApp;

public partial class MainWindow : Window
{
    private WeatherService? _weatherService;
    private readonly SettingsService _settingsService;
    private AppSettings _currentSettings = new();
    
    public MainWindow()
    {
        InitializeComponent();
        _settingsService = new SettingsService();
        LoadSettingsAsync();
    }

    private async void LoadSettingsAsync()
    {
        _currentSettings = await _settingsService.LoadSettingsAsync();
        
        if (string.IsNullOrEmpty(_currentSettings.ApiKey))
        {
            await ShowApiKeyDialog();
        }

        _weatherService = new WeatherService(_currentSettings.ApiKey);
        
        // Apply loaded settings to UI
        DefaultCityBox.Text = _currentSettings.DefaultCity;
        LanguageComboBox.SelectedIndex = GetLanguageIndex(_currentSettings.Language);
        ThemeComboBox.SelectedIndex = GetThemeIndex(_currentSettings.Theme);
        
        // Apply theme immediately
        ApplyTheme(_currentSettings.Theme);

        // If default city is set, load its weather
        if (!string.IsNullOrEmpty(_currentSettings.DefaultCity))
        {
            CitySearchBox.Text = _currentSettings.DefaultCity;
            await LoadWeatherDataAsync(_currentSettings.DefaultCity);
        }
    }

    private async Task ShowApiKeyDialog()
    {
        var dialog = new Window
        {
            Title = "OpenWeatherMap API Key",
            Width = 400,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var layout = new StackPanel { Margin = new Thickness(20) };
        var textBox = new TextBox 
        { 
            Watermark = "Enter your API key...",
            Margin = new Thickness(0, 0, 0, 10)
        };
        var button = new Button 
        { 
            Content = "Save",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        };

        button.Click += async (s, e) =>
        {
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                _currentSettings.ApiKey = textBox.Text;
                await _settingsService.SaveSettingsAsync(_currentSettings);
                dialog.Close();
            }
        };

        layout.Children.Add(new TextBlock 
        { 
            Text = "Please enter your OpenWeatherMap API key:",
            Margin = new Thickness(0, 0, 0, 10)
        });
        layout.Children.Add(textBox);
        layout.Children.Add(button);

        dialog.Content = layout;
        await dialog.ShowDialog(this);
    }

    private async void OnSearchClick(object sender, RoutedEventArgs e)
    {
        var cityName = CitySearchBox?.Text;
        if (string.IsNullOrWhiteSpace(cityName))
        {
            // Show error message to user
            return;
        }
        await LoadWeatherDataAsync(cityName);
    }

    private async Task LoadWeatherDataAsync(string cityName)
    {
        if (_weatherService == null) return;
        
        if (string.IsNullOrWhiteSpace(cityName))
            return;

        var coordinates = await _weatherService.GetCityCoordinatesAsync(cityName);
        if (coordinates == null)
        {
            // Show error message
            return;
        }

        var weatherData = await _weatherService.GetWeatherDataAsync(
            coordinates.Value.Lat, 
            coordinates.Value.Lon,
            _currentSettings.Language);
            
        if (weatherData == null)
        {
            // Show error message
            return;
        }

        // Update UI with weather data
        WeatherPanel.IsVisible = true;
        UpdateWeatherDisplay(weatherData);
    }

    private async void OnSaveSettingsClick(object sender, RoutedEventArgs e)
    {
        var newTheme = GetSelectedTheme();
        var settings = new AppSettings
        {
            DefaultCity = DefaultCityBox.Text ?? string.Empty,
            Language = GetSelectedLanguage(),
            Theme = newTheme,
            ApiKey = _currentSettings.ApiKey
        };

        await _settingsService.SaveSettingsAsync(settings);
        _currentSettings = settings;

        // Apply theme immediately after saving
        ApplyTheme(newTheme);
    }

    private string GetSelectedLanguage()
    {
        var selectedItem = LanguageComboBox.SelectedItem as ComboBoxItem;
        return selectedItem?.Tag?.ToString() ?? "en";
    }

    private string GetSelectedTheme()
    {
        var selectedItem = ThemeComboBox.SelectedItem as ComboBoxItem;
        return selectedItem?.Tag?.ToString() ?? "Default";
    }

    private int GetLanguageIndex(string language)
    {
        return language switch
        {
            "fr" => 1,
            "de" => 2,
            "es" => 3,
            _ => 0 // "en" or default
        };
    }

    private int GetThemeIndex(string theme)
    {
        return theme switch
        {
            "Light" => 1,
            "Dark" => 2,
            _ => 0 // "Default"
        };
    }

    private void ApplyTheme(string theme)
    {
        if (Application.Current is App app)
        {
            var themeVariant = theme switch
            {
                "Light" => ThemeVariant.Light,
                "Dark" => ThemeVariant.Dark,
                _ => ThemeVariant.Default
            };
            
            app.RequestedThemeVariant = themeVariant;
        }
    }

    private async void UpdateWeatherDisplay(WeatherData weatherData)
    {
        Console.WriteLine($"Updating weather display...");
        Console.WriteLine($"Current weather icon code: {weatherData.WeatherIcon}");
        
        // Update city info
        CityNameText.Text = weatherData.CityName;
        CoordinatesText.Text = $"Lat: {weatherData.Latitude:F2}, Lon: {weatherData.Longitude:F2}";
        
        // Update current weather
        if (_weatherService != null)
        {
            Console.WriteLine("Getting weather icon stream...");
            var iconStream = await _weatherService.GetWeatherIconAsync(weatherData.WeatherIcon);
            if (iconStream != null)
            {
                try
                {
                    Console.WriteLine("Creating bitmap from stream...");
                    WeatherIcon.Source = new Avalonia.Media.Imaging.Bitmap(iconStream);
                    Console.WriteLine("Bitmap created and set as source.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating bitmap: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Icon stream was null!");
            }
        }
        
        TemperatureText.Text = $"{weatherData.Temperature:F1}°C";
        DescriptionText.Text = weatherData.Description;
        HumidityText.Text = $"Humidity: {weatherData.Humidity}%";

        // Update forecasts using DataContext
        DailyForecastListDetailed.DataContext = weatherData;

        // Make the panel visible
        WeatherPanel.IsVisible = true;
    }
}