using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace WetterApp
{
    public partial class MainWindow : Window
    {
        private readonly string apiKey = "b9dcbd51c26e372ce680c1e678952f1b";
        private readonly string requestUrl = "https://api.openweathermap.org/data/2.5/weather";

        public MainWindow()
        {
            InitializeComponent();
            UpdateUi("Lambsheim");
        }

        public void UpdateUi(string city)
        {
            try
            {
                WeatherMapResponse result = GetWeatherData(city);

                string finalImage = "Snow.png";
                string currentWeather = result.weather[0].main.ToLower();

                if (currentWeather.Contains("cloud"))
                {
                    finalImage = "Cloud.png";
                }
                else if (currentWeather.Contains("rain"))
                {
                    finalImage = "Rain.png";
                }
                else if (currentWeather.Contains("clear") || currentWeather.Contains("sun"))
                {
                    finalImage = "Sun.png";
                }

                backgroundImage.ImageSource = new BitmapImage(new Uri("Images/" + finalImage, UriKind.Relative));
                labelTemperature.Content = (int)result.main.temp + "°C";
                labelInfo.Content = result.weather[0].main;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Aktualisieren der UI: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public WeatherMapResponse GetWeatherData(string city)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var finalUri = $"{requestUrl}?q={city}&appid={apiKey}&units=metric";
                    HttpResponseMessage httpResponse = httpClient.GetAsync(finalUri).Result;

                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        throw new Exception("Ungültige Stadt oder API-Anfrage fehlgeschlagen.");
                    }

                    string response = httpResponse.Content.ReadAsStringAsync().Result;
                    WeatherMapResponse weatherMapResponse = JsonConvert.DeserializeObject<WeatherMapResponse>(response);

                    return weatherMapResponse;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Abrufen der Wetterdaten: {ex.Message}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = "Lambsheim";
                }
                else
                {
                    string query = textBox.Text;
                    UpdateUi(query);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei der Button-Aktion: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
