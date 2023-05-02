using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
            btnSearch.Click += BtnSearch_Click;
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string pokemonName = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(pokemonName))
            {
                MessageBox.Show("Veuillez saisir le nom d'un Pokémon.");
                return;
            }

            try
            {
                string apiUrl = $"https://pokeapi.co/api/v2/pokemon/{pokemonName}";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(responseBody);

                string name = json["name"].Value<string>();
                JArray types = (JArray)json["types"];
                string type1 = types[0]["type"]["name"].Value<string>();
                string type2 = types.Count > 1 ? types[1]["type"]["name"].Value<string>() : "";
                int hp = json["stats"][0]["base_stat"].Value<int>();
                int attack = json["stats"][1]["base_stat"].Value<int>();
                string spriteUrl = json["sprites"]["front_default"].Value<string>();

                lblName.Content = name;
                lblType.Content = type2 != "" ? $"{type1}/{type2}" : type1;
                lblHP.Content = hp.ToString();
                lblAttack.Content = attack.ToString();

                BitmapImage spriteImage = new BitmapImage(new Uri(spriteUrl));
                pbPokemon.Source = spriteImage;
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Erreur lors de la récupération des données : {ex.Message}");
            }
            catch (JsonException ex)
            {
                MessageBox.Show($"Erreur lors de la désérialisation des données : {ex.Message}");
            }
        }
    }
}
