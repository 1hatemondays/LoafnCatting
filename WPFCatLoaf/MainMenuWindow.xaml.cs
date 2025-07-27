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

            // Redirect table users to their ordering interface
            if (_currentUser.RoleId == 5) // Table users
            {
                var tableOrderWindow = new TableOrderWindow(_currentUser);
                tableOrderWindow.Show();
                this.Close();
                return;
            }
            
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

        private void TableManagement_Click(object sender, MouseButtonEventArgs e)
        {
            if (_currentUser.RoleId == 3 || _currentUser.RoleId == 2)
            {
                var tableWindow = new TableManagementWindow(_currentUser);
                tableWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("You do not have permission to manage tables.", "Access Denied",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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
            // Only Admin (4) and Manager (2) can access reports
            if (_currentUser.RoleId == 4 || _currentUser.RoleId == 2)
            {
                var reportWindow = new ReportWindow(_currentUser);
                reportWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("You do not have permission to view reports.", "Access Denied",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ReservationManagement_Click(object sender, MouseButtonEventArgs e)
        {
            var reservationWindow = new ReservationManagementWindow(_currentUser);
            reservationWindow.Show();
            this.Close();
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
                4 => "Admin",
                2 => "Manager",
                3 => "Staff",
                5 => "Table",
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