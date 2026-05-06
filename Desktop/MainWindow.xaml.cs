using Desktop.AdminWindows;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Desktop
{
    /// <summary>
    /// Interak�n� logika pro MainWindow.xaml
    /// </summary>
    /// 

    public class Token
    {
        public string token { get; set; } = "";
    }
    public partial class MainWindow : Window
    {
        HttpClient client = new HttpClient() { 
            BaseAddress = new Uri("http://localhost:5170/") 
        };
        public string ApiToken { get; set; } = "";

        public MainWindow()
        {
            InitializeComponent();
            MainWindow_Loaded();
        }

        private async void MainWindow_Loaded()
        {
            try
            {
                string loginJson = "{ \"password\": \"admin\" }";
                StringContent loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");

                //request
                HttpResponseMessage loginResponse = await client.PostAsync("api/auth/token", loginContent);
                if (!loginResponse.IsSuccessStatusCode)
                {
                    TextBoxApiChecker.Text = ("Token request failed: " + loginResponse.StatusCode);
                    return;
                }
                //response
                string loginResponseBody = await loginResponse.Content.ReadAsStringAsync();
                Token? tokenResponse = JsonSerializer.Deserialize<Token>(loginResponseBody);
                if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.token))
                {
                    TextBoxApiChecker.Text = "Token nepřijat";
                    return;
                }
                ApiToken = tokenResponse.token;
            }
            catch (Exception ex)
            {
                MessageBox.Show("API err:" + ex.Message);
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiToken);
        }

        private async void ButtonApiChecker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpResponseMessage usersResponse = await client.GetAsync("api/users/AllUsers");

                if (!usersResponse.IsSuccessStatusCode)
                {
                    TextBoxApiChecker.Text = ("Users request failed: " + usersResponse.StatusCode);
                    return;
                }

                string usersJson = await usersResponse.Content.ReadAsStringAsync();

                TextBoxApiChecker.Text =("API Jede");
            }
            catch (Exception ex)
            {
                MessageBox.Show("API err:" + ex.Message);
            }
        }

        private void ButtonUsers_Click(object sender, RoutedEventArgs e)
        {
            UsersWindow window = new UsersWindow(client);
            window.Show();
        }
    }
}
