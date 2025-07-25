using DataAccessLayer.Models;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace WPFCatLoaf
{
    public partial class MainMenuWindow : Window
    {
        private readonly User _currentUser;
        private DispatcherTimer _timer;

        public MainMenuWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            
            SetupWelcomeMessage();
            SetupTimer();
        }

        private void SetupWelcomeMessage()
        {
            WelcomeTextBlock.Text = $"Welcome back, {_currentUser.Name}! Choose an option to manage your cafe.";
        }

        private void SetupTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CurrentTimeTextBlock.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy - HH:mm:ss");
        }

        #region Menu Item Click Handlers

        private void ProductManagement_Click(object sender, MouseButtonEventArgs e)
        {
            var productWindow = new ProductManagementWindow(_currentUser);
            productWindow.Show();
            this.Close();
        }

        private void OrderManagement_Click(object sender, MouseButtonEventArgs e)
        {
            var orderWindow = new OrderManagementWindow(_currentUser);
            orderWindow.Show();
            this.Close();
        }

        private void UserManagement_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("User Management feature coming soon!", "Feature Not Available", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CatManagement_Click(object sender, MouseButtonEventArgs e)
        {
            var catWindow = new CatManagementWindow(_currentUser);
            catWindow.Show();
            this.Close();
        }

        private void Reports_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Reports & Analytics feature coming soon!", "Feature Not Available", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Settings_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Settings feature coming soon!", "Feature Not Available", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Button Click Handlers

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Profile Information:\n\nName: {_currentUser.Name}\nEmail: {_currentUser.Email}\nRole: {GetRoleName(_currentUser.RoleId)}", 
                "User Profile", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Logout Confirmation", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _timer?.Stop();
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        #endregion

        private string GetRoleName(int roleId)
        {
            return roleId switch
            {
                1 => "Admin",
                2 => "Manager",
                3 => "Staff",
                _ => "Unknown"
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer?.Stop();
            base.OnClosed(e);
        }
    }
}