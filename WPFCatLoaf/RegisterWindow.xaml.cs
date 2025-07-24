using BusinessLayer.IService;
using DataAccessLayer.Models;
using System;
using System.Windows;

namespace WPFCatLoaf
{
    public partial class RegisterWindow : Window
    {
        private readonly IUserService _userService;

        public RegisterWindow(IUserService userService)
        {
            InitializeComponent();
            _userService = userService;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Passwords do not match.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newUser = new User
            {
                Name = NameTextBox.Text,
                Email = EmailTextBox.Text,
                Password = PasswordBox.Password,
                PhoneNumber = PhoneNumberTextBox.Text,
                RoleId = 2 
            };

            if (_userService.AddUser(newUser))
            {
                MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Registration failed. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}