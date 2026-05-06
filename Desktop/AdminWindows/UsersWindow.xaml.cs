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

            foreach (var user in users)
            {
                Users.Add(user);
            }
        }
        priva
        private List<DesktopUserDto> GetSelectedItems()
        {
            return DataGridItems.SelectedItems.Cast<DesktopUserDto>().ToList();
        }

        private async void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            await FetchAllUsers();
        }
    }
}
