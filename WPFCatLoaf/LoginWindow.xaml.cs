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
            CreateTestDataIfNeeded(context);
            
        }

        // Development method to create test data
        private void CreateTestDataIfNeeded(LoafNcattingDbContext context)
        {
            try
            {
                // Check if any users exist
                if (!context.Users.Any())
                {

                    // Create test users
                    var testUsers = new[]
                    {
                        new User 
                        { 
                            Name = "Admin User", 
                            Email = "admin@catcafe.com", 
                            Password = "admin123", 
                            PhoneNumber = "0123456789",
                            RoleId = 2
                        },
                        new User 
                        { 
                            Name = "Staff User", 
                            Email = "staff@catcafe.com", 
                            Password = "staff123", 
                            PhoneNumber = "0987654321",
                            RoleId = 3
                        },
                        new User 
                        { 
                            Name = "Test Customer", 
                            Email = "customer@test.com", 
                            Password = "test123", 
                            PhoneNumber = "0555666777",
                            RoleId = 1
                        }
                    };

                    context.Users.AddRange(testUsers);
                    context.SaveChanges();

                    MessageBox.Show("Đã tạo dữ liệu test:\n\n" +
                                  "Admin: admin@catcafe.com / admin123\n" +
                                  "Staff: staff@catcafe.com / staff123\n" +
                                  "Customer: customer@test.com / test123", 
                                  "Test Data Created", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                // Ignore errors in development data creation
                Console.WriteLine($"Error creating test data: {ex.Message}");
            }
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
                    // Login successful
                    

                    // MỞ CỬA SỔ TƯƠNG ỨNG VỚI VAI TRÒ
                    if (user.RoleId == 2 || user.RoleId==3)
                    {
                        var orderManagementWindow = new OrderWindow(user);
                        orderManagementWindow.Show();
                        this.Close(); // Đóng cửa sổ đăng nhập
                    }
                    else
                    {
                        // Mở cửa sổ chính hoặc cửa sổ admin ở đây nếu có
                        // var mainWindow = new MainWindow(user);
                        // mainWindow.Show();
                        // this.Close();
                    }
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
