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
        
        DefaultCityBox.Text = _currentSettings.DefaultCity;
        LanguageComboBox.SelectedIndex = GetLanguageIndex(_currentSettings.Language);
        ThemeComboBox.SelectedIndex = GetThemeIndex(_currentSettings.Theme);
        
        ApplyTheme(_currentSettings.Theme);

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
            Width = 600,
            Height = 320,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Background = new SolidColorBrush(Colors.Transparent),
            SystemDecorations = SystemDecorations.None,
            CanResize = false
        };

        var mainBorder = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(245, 28, 28, 30)),
            CornerRadius = new CornerRadius(16),
            BoxShadow = new BoxShadows(new BoxShadow
            {
                OffsetX = 0,
                OffsetY = 8,
                Blur = 32,
                Color = Color.FromArgb(80, 0, 0, 0)
            }),
            Margin = new Thickness(20)
        };

        var layout = new StackPanel 
        { 
            Margin = new Thickness(32),
            Spacing = 20
        };

        var titleBlock = new TextBlock
        {
            Text = "Welcome to WeatherApp",
            FontSize = 24,
            FontWeight = FontWeight.SemiBold,
            Foreground = new SolidColorBrush(Colors.White),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 8)
        };

        var subtitleBlock = new TextBlock
        {
            Text = "Please enter your OpenWeatherMap API key to get started:",
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Center,
            Foreground = new SolidColorBrush(Color.Parse("#999999")),
            Margin = new Thickness(0, 0, 0, 16)
        };

        var textBox = new TextBox
        {
            Watermark = "Enter your API key...",
            Height = 50,
            CornerRadius = new CornerRadius(12),
            BorderBrush = new SolidColorBrush(Color.Parse("#333333")),
            Background = new SolidColorBrush(Color.Parse("#1C1C1E")),
            Foreground = new SolidColorBrush(Colors.White),
            Margin = new Thickness(0, 0, 0, 16)
        };

        var button = new Button
        {
            Content = "Continue",
            HorizontalAlignment = HorizontalAlignment.Center,
            Width = 200,
            Height = 44,
            CornerRadius = new CornerRadius(22),
            Background = new SolidColorBrush(Color.Parse("#0A84FF")),
            Foreground = new SolidColorBrush(Colors.White),
            FontWeight = FontWeight.SemiBold,
            Margin = new Thickness(0, -8, 0, 0),
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center
        };

        var linkPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 16, 0, 0)
        };

        var linkText = new TextBlock
        {
            Text = "Don't have an API key? ",
            Foreground = new SolidColorBrush(Color.Parse("#999999")),
            VerticalAlignment = VerticalAlignment.Center
        };

        var link = new Button
        {
            Content = "Get one here",
            Foreground = new SolidColorBrush(Color.Parse("#0A84FF")),
            Background = new SolidColorBrush(Colors.Transparent),
            Padding = new Thickness(4, 0),
            VerticalAlignment = VerticalAlignment.Center
        };

        link.Click += (s, e) =>
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://openweathermap.org/api",
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        };

        button.Click += async (s, e) =>
        {
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                _currentSettings.ApiKey = textBox.Text;
                await _settingsService.SaveSettingsAsync(_currentSettings);
                dialog.Close();
            }
            else
            {
                textBox.BorderBrush = new SolidColorBrush(Color.Parse("#FF3B30"));
            }
        };

        linkPanel.Children.Add(linkText);
        linkPanel.Children.Add(link);

        layout.Children.Add(titleBlock);
        layout.Children.Add(subtitleBlock);
        layout.Children.Add(textBox);
        layout.Children.Add(button);
        layout.Children.Add(linkPanel);

        mainBorder.Child = layout;
        dialog.Content = mainBorder;

        await dialog.ShowDialog(this);
    }

    private async void OnSearchClick(object sender, RoutedEventArgs e)
    {
        var cityName = CitySearchBox?.Text;
        if (string.IsNullOrWhiteSpace(cityName))
        {
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
            return;
        }

        var weatherData = await _weatherService.GetWeatherDataAsync(
            coordinates.Value.Lat, 
            coordinates.Value.Lon,
            _currentSettings.Language);
            
        if (weatherData == null)
        {
            return;
        }

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
            _ => 0 
        };
    }

    private int GetThemeIndex(string theme)
    {
        return theme switch
        {
            "Light" => 1,
            "Dark" => 2,
            _ => 0 
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
        
        CityNameText.Text = weatherData.CityName;
        CoordinatesText.Text = $"Lat: {weatherData.Latitude:F2}, Lon: {weatherData.Longitude:F2}";
        
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

        DailyForecastListDetailed.DataContext = weatherData;

        WeatherPanel.IsVisible = true;
    }
}