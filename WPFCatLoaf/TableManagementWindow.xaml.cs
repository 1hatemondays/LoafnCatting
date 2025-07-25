using BusinessLayer.IService;
using BusinessLayer.Service;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WPFCatLoaf
{
    public partial class TableManagementWindow : Window
    {
        private readonly IRestaurantTableService _tableService;
        private readonly IUserService _userService;
        private readonly User _loggedInUser;
        private ObservableCollection<RestaurantTable> _allTables;
        private ObservableCollection<RestaurantTable> _filteredTables;
        private RestaurantTable _selectedTable;
        private bool _isEditMode = false;

        public TableManagementWindow(User user)
        {
            InitializeComponent();
            _loggedInUser = user;

            // Initialize services
            var context = new LoafNcattingDbContext();
            _tableService = new RestaurantTableService(new RestaurantTableRepository(context));
            _userService = new UserService(new UserRepository(context));

            // Initialize collections
            _allTables = new ObservableCollection<RestaurantTable>();
            _filteredTables = new ObservableCollection<RestaurantTable>();

            LoadInitialData();
            SetupDataGrid();
        }

        private void LoadInitialData()
        {
            try
            {
                // Load tables
                var tables = _tableService.GetAllRestaurantTables();
                _allTables.Clear();
                foreach (var table in tables)
                {
                    _allTables.Add(table);
                }

                // Load table statuses (you may need to create this service)
                // TableStatusComboBox.ItemsSource = _tableStatusService.GetAllTableStatuses();

                RefreshTablesList();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error loading data: {ex.Message}");
            }
        }

        private void SetupDataGrid()
        {
            TablesDataGrid.ItemsSource = _filteredTables;
        }

        private void RefreshTablesList()
        {
            var searchText = SearchTableTextBox.Text?.ToLower() ?? "";

            _filteredTables.Clear();

            var filteredTables = _allTables.Where(t =>
                string.IsNullOrEmpty(searchText) ||
                t.TableName.ToLower().Contains(searchText)
            );

            foreach (var table in filteredTables)
            {
                _filteredTables.Add(table);
            }
        }

        private void SearchTableTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshTablesList();
        }

        private void TablesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TablesDataGrid.SelectedItem is RestaurantTable selectedTable)
            {
                LoadTableToForm(selectedTable);
            }
        }

        private void LoadTableToForm(RestaurantTable table)
        {
            _selectedTable = table;
            TableNameTextBox.Text = table.TableName;
            TableStatusComboBox.SelectedValue = table.TableStatusId;
        }

        private void EditTableButton_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is RestaurantTable table)
            {
                LoadTableToForm(table);
                _isEditMode = true;
                FormTitleTextBlock.Text = "Edit Table";
                SaveTableButton.Content = "Update Table";
            }
        }

        private void CreateTableAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is RestaurantTable table)
            {
                _selectedTable = table;
                AccountTableNameTextBox.Text = table.TableName;
                AccountNameTextBox.Text = $"Table {table.TableName}";
                AccountEmailTextBox.Text = $"table{table.TableId}@catloaf.com";
                AccountPasswordBox.Password = "table123"; // Default password
                AccountCreationOverlay.Visibility = Visibility.Visible;
            }
        }

        private void SaveTableButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateTableForm())
            {
                try
                {
                    var table = CreateTableFromForm();

                    if (_isEditMode)
                    {
                        // Update table logic would go here if you have UpdateTable service method
                        // if (_tableService.UpdateTable(table))
                        // {
                        //     UpdateTableInCollection(table);
                        //     ShowSuccessMessage("Table updated successfully!");
                        //     ClearTableForm();
                        // }
                        // else
                        // {
                        //     ShowErrorMessage("Failed to update table.");
                        // }
                        ShowErrorMessage("Table update functionality not implemented yet.");
                    }
                    else
                    {
                        // Add new table logic would go here if you have AddTable service method
                        // if (_tableService.AddTable(table))
                        // {
                        //     LoadInitialData();
                        //     ShowSuccessMessage("Table added successfully!");
                        //     ClearTableForm();
                        // }
                        // else
                        // {
                        //     ShowErrorMessage("Failed to add table.");
                        // }
                        ShowErrorMessage("Table creation functionality not implemented yet.");
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage($"Error saving table: {ex.Message}");
                }
            }
        }

        private void CancelTableButton_Click(object sender, RoutedEventArgs e)
        {
            ClearTableForm();
        }

        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateAccountForm())
            {
                try
                {
                    var newUser = new User
                    {
                        Name = AccountNameTextBox.Text.Trim(),
                        Email = AccountEmailTextBox.Text.Trim(),
                        Password = AccountPasswordBox.Password,
                        PhoneNumber = "N/A", // Table accounts don't need phone numbers
                        RoleId = 5 // Table role
                    };

                    if (_userService.AddUser(newUser))
                    {
                        ShowSuccessMessage($"Table account created successfully!\nEmail: {newUser.Email}\nPassword: {AccountPasswordBox.Password}");
                        AccountCreationOverlay.Visibility = Visibility.Collapsed;
                        ClearAccountForm();
                    }
                    else
                    {
                        ShowErrorMessage("Failed to create table account. Email might already exist.");
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage($"Error creating account: {ex.Message}");
                }
            }
        }

        private void CancelAccountButton_Click(object sender, RoutedEventArgs e)
        {
            AccountCreationOverlay.Visibility = Visibility.Collapsed;
            ClearAccountForm();
        }

        private RestaurantTable CreateTableFromForm()
        {
            var table = new RestaurantTable
            {
                TableName = TableNameTextBox.Text.Trim(),
                TableStatusId = (int)TableStatusComboBox.SelectedValue
            };

            if (_isEditMode && _selectedTable != null)
            {
                table.TableId = _selectedTable.TableId;
            }

            return table;
        }

        private bool ValidateTableForm()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(TableNameTextBox.Text))
                errors.Add("Table name is required.");

            if (TableStatusComboBox.SelectedValue == null)
                errors.Add("Table status is required.");

            if (errors.Any())
            {
                ValidationMessageTextBlock.Text = string.Join("\n", errors);
                ValidationMessageTextBlock.Visibility = Visibility.Visible;
                return false;
            }

            ValidationMessageTextBlock.Visibility = Visibility.Collapsed;
            return true;
        }

        private bool ValidateAccountForm()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(AccountNameTextBox.Text))
                errors.Add("Account name is required.");

            if (string.IsNullOrWhiteSpace(AccountEmailTextBox.Text))
                errors.Add("Email is required.");

            if (string.IsNullOrWhiteSpace(AccountPasswordBox.Password))
                errors.Add("Password is required.");

            if (errors.Any())
            {
                AccountValidationMessage.Text = string.Join("\n", errors);
                AccountValidationMessage.Visibility = Visibility.Visible;
                return false;
            }

            AccountValidationMessage.Visibility = Visibility.Collapsed;
            return true;
        }

        private void ClearTableForm()
        {
            _selectedTable = null;
            _isEditMode = false;
            FormTitleTextBlock.Text = "Add New Table";
            SaveTableButton.Content = "Save Table";

            TableNameTextBox.Text = "";
            TableStatusComboBox.SelectedIndex = -1;

            ValidationMessageTextBlock.Visibility = Visibility.Collapsed;
            TablesDataGrid.SelectedItem = null;
        }

        private void ClearAccountForm()
        {
            AccountNameTextBox.Text = "";
            AccountEmailTextBox.Text = "";
            AccountPasswordBox.Password = "";
            AccountValidationMessage.Visibility = Visibility.Collapsed;
        }

        private void BackToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var mainMenuWindow = new MainMenuWindow(_loggedInUser);
            mainMenuWindow.Show();
            this.Close();
        }

        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}