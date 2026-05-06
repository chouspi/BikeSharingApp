using Desktop.Dtos;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Http;
using System.Net;
using System.Net.Http.Json;

namespace Desktop.AdminWindows
{
    /// <summary>
    /// Interakční logika pro UsersWindow.xaml
    /// </summary>
    public partial class UsersWindow : Window, INotifyPropertyChanged
    {
        private DesktopUserDto? selectedUser;

        public ObservableCollection<DesktopUserDto> Users { get; set; } = new ObservableCollection<DesktopUserDto>();

        public DesktopUserDto? SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedUser"));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;


        HttpClient client;
        public UsersWindow(HttpClient client)
        {
            this.client = client;
            InitializeComponent();
            DataContext = this;
            _ = FetchAllUsers();

        }

        private async Task FetchAllUsers()
        {
            var users = await client.GetFromJsonAsync<List<DesktopUserDto>>("api/users/AllUsers");

            Users.Clear();

            if (users == null)
            {
                return;
            }

            foreach (var user in users)
            {
                Users.Add(user);
            }
        }

        private List<DesktopUserDto> GetSelectedItems()
        {
            return DataGridItems.SelectedItems.Cast<DesktopUserDto>().ToList();
        }

        private async void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            await FetchAllUsers();
        }

        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            DesktopUserDto user = new DesktopUserDto();
            user.FirstName = TextBoxFirstName.Text;
            user.LastName = TextBoxLastName.Text;
            user.Email = TextBoxEmail.Text;

            var request = new
            {
                User = user,
                Password = PasswordBoxPassword.Password
            };

            var response = await client.PostAsJsonAsync("api/users/UserAdd", request);

            if (!response.IsSuccessStatusCode)
            {
                TextBlockInfo.Text = await response.Content.ReadAsStringAsync();
                return;
            }

            DesktopUserDto? createdUser = await response.Content.ReadFromJsonAsync<DesktopUserDto>();

            if (createdUser != null)
            {
                Users.Add(createdUser);
                SelectedUser = createdUser;
                TextBlockInfo.Text = "Uzivatel pridan.";
                PasswordBoxPassword.Password = "";
            }
        }
    }
}
