using System;
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
using BusinessLayer.Service;
using DataAccessLayer.Repository;
using DataAccessLayer.Models;

namespace WPFCatLoaf
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly UserService _userService;

        public LoginWindow()
        {
            InitializeComponent();
            
            var context = new LoafNcattingDbContext();
            var userRepository = new UserRepository(context);
            _userService = new UserService(userRepository);
            
            // Create test data if needed (for development only)
            
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await PerformLogin();
        }

        private async Task PerformLogin()
        {
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                ShowErrorMessage("Vui lòng nhập địa chỉ email của bạn.");
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                ShowErrorMessage("Vui lòng nhập mật khẩu của bạn.");
                return;
            }

            ShowLoading(true);

            try
            {
                await Task.Delay(1000);

                // Attempt login
                var user = _userService.GetUserByEmailAndPassword(EmailTextBox.Text.Trim(), PasswordBox.Password);
                if (user != null)
                {
                    // Login successful - All roles can access main menu
                    var mainMenuWindow = new MainMenuWindow(user);
                    mainMenuWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Đã xảy ra lỗi khi đăng nhập: {ex.Message}");
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ShowLoading(bool isLoading)
        {
            LoadingOverlay.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            LoginButton.IsEnabled = !isLoading;
            EmailTextBox.IsEnabled = !isLoading;
            PasswordBox.IsEnabled = !isLoading;
        }

        // Handle Enter key press for quick login
        private void EmailTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PasswordBox.Focus();
            }
        }

        private async void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await PerformLogin();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow(_userService);
            registerWindow.ShowDialog();
        }
    }
}
