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
using System.Windows.Media.Animation;
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
        private bool _isAccountMode = true; // true for account login, false for table ID

        public LoginWindow()
        {
            InitializeComponent();
            
            var context = new LoafNcattingDbContext();
            var userRepository = new UserRepository(context);
            _userService = new UserService(userRepository);
            
            // Initialize in account mode
            SetAccountMode(true);
        }

        private void AccountToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isAccountMode)
            {
                SetAccountMode(true);
                AnimateToggle(false); // Move to left (account mode)
            }
        }

        private void TableToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isAccountMode)
            {
                SetAccountMode(false);
                AnimateToggle(true); // Move to right (table mode)
            }
        }

        private void SetAccountMode(bool isAccountMode)
        {
            _isAccountMode = isAccountMode;
            
            if (isAccountMode)
            {
                // Account mode
                AccountLoginPanel.Visibility = Visibility.Visible;
                TableIdPanel.Visibility = Visibility.Collapsed;
                RegisterButton.Visibility = Visibility.Visible;
                LoginButton.Content = "SIGN IN";
                
                // Update toggle button colors
                AccountToggleButton.Foreground = Brushes.White;
                TableToggleButton.Foreground = Brushes.Gray;
            }
            else
            {
                // Table ID mode
                AccountLoginPanel.Visibility = Visibility.Collapsed;
                TableIdPanel.Visibility = Visibility.Visible;
                RegisterButton.Visibility = Visibility.Collapsed;
                LoginButton.Content = "START ORDERING";
                
                // Update toggle button colors
                AccountToggleButton.Foreground = Brushes.Gray;
                TableToggleButton.Foreground = Brushes.White;
            }
        }

        private void AnimateToggle(bool moveRight)
        {
            var animation = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase()
            };

            if (moveRight)
            {
                // Move to table mode (right side)
                animation.To = AccountToggleButton.ActualWidth;
            }
            else
            {
                // Move to account mode (left side)
                animation.To = 0;
            }

            ToggleTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isAccountMode)
            {
                await PerformAccountLogin();
            }
            else
            {
                await PerformTableLogin();
            }
        }

        private async Task PerformAccountLogin()
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
                else
                {
                    ShowErrorMessage("Email hoặc mật khẩu không đúng.");
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

        private async Task PerformTableLogin()
        {
            if (string.IsNullOrWhiteSpace(TableIdTextBox.Text))
            {
                ShowErrorMessage("Vui lòng nhập số bàn của bạn.");
                return;
            }

            if (!int.TryParse(TableIdTextBox.Text.Trim(), out int tableId))
            {
                ShowErrorMessage("Số bàn phải là một số nguyên hợp lệ.");
                return;
            }

            ShowLoading(true);

            try
            {
                await Task.Delay(800);

                // Open TableOrderWindow with table ID for customer
                var tableOrderWindow = new TableOrderWindow(tableId);
                tableOrderWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Đã xảy ra lỗi khi truy cập bàn: {ex.Message}");
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
            TableIdTextBox.IsEnabled = !isLoading;
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
                await PerformAccountLogin();
            }
        }

        private async void TableIdTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await PerformTableLogin();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow(_userService);
            registerWindow.ShowDialog();
        }
    }
}
