using BusinessLayer.IService;
using BusinessLayer.Service;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace WPFCatLoaf
{
    public partial class StaffManagementWindow : Window
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly User _loggedInUser;
        private ObservableCollection<User> _allStaff = new();
        private ObservableCollection<User> _filteredStaff = new();
        private User _selectedStaff;
        private bool _isEditMode = false;

        // Constants
        private const int STAFF_ROLE_ID = 3;
        private const int MANAGER_ROLE_ID = 2;
        private const int ADMIN_ROLE_ID = 4;

        public StaffManagementWindow(User user)
        {
            InitializeComponent();
            _loggedInUser = user;

            var context = new LoafNcattingDbContext();
            _userService = new UserService(new UserRepository(context));
            _roleService = new RoleService(new RoleRepository(context));

            CheckUserPermissions();
            InitializeUI();
        }

        #region Initialization

        private void InitializeUI()
        {
            LoadInitialData();
            LoadRoles();
            SetupDataGrid();
        }

        private void CheckUserPermissions()
        {
            if (_loggedInUser.RoleId == 4)
            {
                WelcomeTextBlock.Text = $"Welcome, {_loggedInUser.Name}! Manage staff & managers.";
                EnableFormEditing();
            }
            else if (_loggedInUser.RoleId == 2)
            {
                WelcomeTextBlock.Text = $"Welcome, {_loggedInUser.Name}! View cafe staff members.";
                DisableFormEditing();
            }
            else
            {
                RedirectToMainMenu("You do not have permission to access this area.");
            }
        }

        private void SetupDataGrid() => StaffDataGrid.ItemsSource = _filteredStaff;

        private void LoadInitialData()
        {
            try
            {
                var usersToManage = GetUsersForCurrentRole();
                usersToManage = usersToManage.Where(u => u.UserId != _loggedInUser.UserId).ToList();

                _allStaff.Clear();
                foreach (var user in usersToManage.OrderBy(u => u.Name))
                {
                    _allStaff.Add(user);
                }

                RefreshStaffList();
                UpdateStaffCount();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error loading data: {ex.Message}");
            }
        }

        private List<User> GetUsersForCurrentRole()
        {
            var users = new List<User>();

            if (_loggedInUser.RoleId == ADMIN_ROLE_ID)
            {
                users.AddRange(_userService.GetAllUsersByRoleId(3));
                users.AddRange(_userService.GetAllUsersByRoleId(2));
            }
            else if (_loggedInUser.RoleId == 2)
            {
                users.AddRange(_userService.GetAllUsersByRoleId(3));
            }

            return users;
        }

        private void LoadRoles()
        {
            try
            {
                var roles = _roleService.GetAllRoles()
                    .Where(r => r.RoleId == STAFF_ROLE_ID || r.RoleId == MANAGER_ROLE_ID)
                    .ToList();

                RoleComboBox.ItemsSource = roles;
                RoleComboBox.DisplayMemberPath = "RoleName";
                RoleComboBox.SelectedValuePath = "RoleId";
                RoleComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error loading roles: {ex.Message}");
            }
        }

        #endregion

        #region UI State Management

        private void DisableFormEditing()
        {
            FormTitleTextBlock.Text = "Staff Details (View Only)";

            SaveButton.Visibility=Visibility.Collapsed;
            ClearButton.Visibility = Visibility.Visible;
            NewStaffButton.Visibility = Visibility.Collapsed;
            NameTextBox.IsEnabled = false;
            EmailTextBox.IsEnabled = false;
            PhoneTextBox.IsEnabled = false;
            PasswordBox.IsEnabled = false;
            PasswordBox.Visibility=Visibility.Collapsed;
            RoleComboBox.IsEnabled = false;
        }

        private void EnableFormEditing()
        {
            // Enable form controls
            SaveButton.Visibility = Visibility.Visible;
            ClearButton.Visibility = Visibility.Visible;
            NewStaffButton.Visibility = Visibility.Visible;
            NameTextBox.IsEnabled = true;
            EmailTextBox.IsEnabled = true;
            PhoneTextBox.IsEnabled = true;
            PasswordBox.IsEnabled = true;
            PasswordBox.Visibility=Visibility.Visible;
            RoleComboBox.IsEnabled = true;
        }

        private void RedirectToMainMenu(string message = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show(message, "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            var mainMenuWindow = new MainMenuWindow(_loggedInUser);
            mainMenuWindow.Show();
            this.Close();
        }

        #endregion

        #region Staff List Operations

        private void RefreshStaffList()
        {
            var searchText = SearchTextBox?.Text?.ToLower() ?? "";

            _filteredStaff.Clear();

            var filteredUsers = _allStaff.Where(s =>
                string.IsNullOrEmpty(searchText) ||
                s.Name.ToLower().Contains(searchText) ||
                s.Email.ToLower().Contains(searchText) ||
                (s.PhoneNumber != null && s.PhoneNumber.ToLower().Contains(searchText))
            );

            foreach (var staff in filteredUsers)
            {
                _filteredStaff.Add(staff);
            }

            UpdateStaffCount();
        }

        private void UpdateStaffCount() =>
            StaffCountTextBlock.Text = $"Total Staff: {_filteredStaff.Count}";

        #endregion

        #region Form Operations

        private void LoadStaffToForm(User staff)
        {
            if (staff == null) return;

            _selectedStaff = staff;

            NameTextBox.Text = staff.Name;
            EmailTextBox.Text = staff.Email;
            PhoneTextBox.Text = staff.PhoneNumber;
            PasswordBox.Text=staff.Password;

            // Find and select the staff's role
            foreach (Role role in RoleComboBox.Items)
            {
                if (role.RoleId == staff.RoleId)
                {
                    RoleComboBox.SelectedItem = role;
                    break;
                }
            }

            _isEditMode = true;
            FormTitleTextBlock.Text = $"Edit Staff: {staff.Name}";
            SaveButton.Content = "Update Staff";
        }

        private User CreateStaffFromForm()
        {
            var selectedRole = RoleComboBox.SelectedItem as Role;

            var staff = new User
            {
                Name = NameTextBox.Text.Trim(),
                Email = EmailTextBox.Text.Trim(),
                PhoneNumber = PhoneTextBox.Text.Trim(),
                RoleId = selectedRole?.RoleId ?? STAFF_ROLE_ID
            };

            if (_isEditMode && _selectedStaff != null)
            {
                staff.UserId = _selectedStaff.UserId;
                staff.Password = string.IsNullOrEmpty(PasswordBox.Text)
                               ? _selectedStaff.Password
                               : PasswordBox.Text;
            }
            else
            {
                staff.Password = PasswordBox.Text;
            }

            return staff;
        }

        private bool ValidateForm()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                errors.Add("Name is required.");

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
                errors.Add("Email is required.");
            else if (!IsValidEmail(EmailTextBox.Text))
                errors.Add("Please enter a valid email address.");

            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
                errors.Add("Phone number is required.");
            else if (!IsValidPhoneNumber(PhoneTextBox.Text))
                errors.Add("Please enter a valid phone number.");

            if (RoleComboBox.SelectedItem == null)
                errors.Add("Please select a role.");

            if (IsEmailDuplicate())
                errors.Add("This email is already in use by another staff/manager.");

            ValidatePassword(errors);

            if (errors.Any())
            {
                ValidationMessageTextBlock.Text = string.Join("\n", errors);
                ValidationMessageTextBlock.Visibility = Visibility.Visible;
                return false;
            }

            ValidationMessageTextBlock.Visibility = Visibility.Collapsed;
            return true;
        }

        private bool IsEmailDuplicate()
        {
            if (!_isEditMode || (_selectedStaff != null && !EmailTextBox.Text.Equals(_selectedStaff.Email, StringComparison.OrdinalIgnoreCase)))
            {
                var allUsersToCheck = new List<User>();
                allUsersToCheck.AddRange(_userService.GetAllUsersByRoleId(STAFF_ROLE_ID));
                allUsersToCheck.AddRange(_userService.GetAllUsersByRoleId(MANAGER_ROLE_ID));

                var existingUser = allUsersToCheck
                    .FirstOrDefault(u => u.Email.Equals(EmailTextBox.Text.Trim(), StringComparison.OrdinalIgnoreCase));

                return existingUser != null && (!_isEditMode || existingUser.UserId != _selectedStaff.UserId);
            }
            return false;
        }

        private void ValidatePassword(List<string> errors)
        {
            if (!_isEditMode && string.IsNullOrWhiteSpace(PasswordBox.Text))
            {
                errors.Add("Password is required for new staff members.");
            }
        }

        private void ClearForm()
        {
            _selectedStaff = null;
            _isEditMode = false;
            FormTitleTextBlock.Text = "Add New Staff";
            SaveButton.Content = "Save Staff";

            // Clear form fields
            NameTextBox.Text = "";
            EmailTextBox.Text = "";
            PhoneTextBox.Text = "";
            PasswordBox.Text = "";

            // Reset role selection
            if (RoleComboBox.Items.Count > 0)
                RoleComboBox.SelectedIndex = 0;

            ValidationMessageTextBlock.Visibility = Visibility.Collapsed;
            StaffDataGrid.SelectedItem = null;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                return new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$").IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phone)
        {
            try
            {
                return new Regex(@"^[\d\s\(\)\-\+]+$").IsMatch(phone) && phone.Any(char.IsDigit);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Event Handlers

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
            RefreshStaffList();

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            RefreshStaffList();
        }

        private void StaffDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StaffDataGrid.SelectedItem is User selectedStaff)
            {
                _isEditMode = true;
                LoadStaffToForm(selectedStaff);
            }
        }

        private void DeleteStaffButton_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInUser.RoleId != ADMIN_ROLE_ID)
            {
                ShowErrorMessage("You do not have permission to delete staff members.");
                return;
            }

            if (((FrameworkElement)sender).DataContext is User staff)
            {
                if (staff.UserId == _loggedInUser.UserId)
                {
                    ShowErrorMessage("You cannot delete your own account.");
                    return;
                }

                if (ConfirmDeletion(staff.Name))
                {
                    DeleteStaffMember(staff);
                }
            }
        }

        private bool ConfirmDeletion(string staffName)
        {
            return MessageBox.Show(
                $"Are you sure you want to delete staff member '{staffName}'?",
                "Delete Staff",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        private void DeleteStaffMember(User staff)
        {
            try
            {
                if (_userService.DeleteUser(staff.UserId))
                {
                    _allStaff.Remove(staff);
                    RefreshStaffList();
                    ClearForm();
                    ShowSuccessMessage("Staff deleted successfully!");
                }
                else
                {
                    ShowErrorMessage("Failed to delete staff member.");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error deleting staff: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInUser.RoleId != ADMIN_ROLE_ID)
            {
                ShowErrorMessage("You do not have permission to add or edit staff members.");
                return;
            }

            if (ValidateForm())
            {
                SaveStaffMember();
            }
        }

        private void SaveStaffMember()
        {
            try
            {
                var staff = CreateStaffFromForm();

                if (_isEditMode)
                {
                    UpdateExistingStaff(staff);
                }
                else
                {
                    AddNewStaff(staff);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error saving staff: {ex.Message}");
            }
        }

        private void UpdateExistingStaff(User staff)
        {
            if (_userService.UpdateUser(staff))
            {
                LoadInitialData();
                ShowSuccessMessage("Staff updated successfully!");
                ClearForm();
            }
            else
            {
                ShowErrorMessage("Failed to update staff member.");
            }
        }

        private void AddNewStaff(User staff)
        {
            if (_userService.AddUser(staff))
            {
                _allStaff.Add(staff);
                RefreshStaffList();
                ShowSuccessMessage("Staff added successfully!");
                ClearForm();
            }
            else
            {
                ShowErrorMessage("Failed to add staff member.");
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void NewStaffButton_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void BackToMenuButton_Click(object sender, RoutedEventArgs e) => RedirectToMainMenu();

        #endregion

        #region Helper Methods

        private void ShowSuccessMessage(string message) =>
            MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        private void ShowErrorMessage(string message) =>
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        #endregion

      
    }
}