using BusinessLayer.IService;
using BusinessLayer.Service;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace WPFCatLoaf
{
    public partial class TableManagementWindow : Window
    {
        private readonly IRestaurantTableService _tableService;
        private readonly User _loggedInUser;
        private Dictionary<string, RestaurantTable> _tableData;
        private RestaurantTable _selectedTable;

        public TableManagementWindow(User user)
        {
            InitializeComponent();
            _loggedInUser = user;

            // Initialize service
            var context = new LoafNcattingDbContext();
            _tableService = new RestaurantTableService(new RestaurantTableRepository(context));
            
            // Initialize table data dictionary
            _tableData = new Dictionary<string, RestaurantTable>();
            
            // Load tables
            LoadTables();
        }

        private void LoadTables()
        {
            try
            {
                // Get all tables from database
                var tables = _tableService.GetAllRestaurantTables();
                _tableData.Clear();
                
                // Find all table buttons in the UI
                foreach (string tableName in new[] { "A1", "A2", "A3", "A4", "A5", "B1", "B2", "B3", "B4", "B5" })
                {
                    // Find table in database
                    RestaurantTable table = tables.FirstOrDefault(t => t.TableName == tableName);
                    
                    // Get the table button
                    Button tableButton = FindName("Table" + tableName) as Button;
                    
                    if (tableButton != null)
                    {
                        if (table != null)
                        {
                            // Store table data and update button appearance
                            _tableData[tableName] = table;
                            UpdateTableButtonAppearance(tableButton, table);
                        }
                        else
                        {
                            // Table not in database, disable the button
                            tableButton.IsEnabled = false;
                            tableButton.Background = Brushes.LightGray;
                            tableButton.Foreground = Brushes.Gray;
                            tableButton.ToolTip = "Table not available";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tables: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void UpdateTableButtonAppearance(Button button, RestaurantTable table)
        {
            SolidColorBrush background;
            SolidColorBrush textColor = Brushes.Black;
            string tooltip = "";
            
            // Set color based on status ID
            switch (table.TableStatusId)
            {
                case 1: // Empty (Trống)
                    background = (SolidColorBrush)FindResource("EmptyTableColor");
                    tooltip = "Status: Trống";
                    break;
                    
                case 2: // In Use (Đang được sử dụng)
                    background = (SolidColorBrush)FindResource("InUseTableColor");
                    textColor = Brushes.White;
                    tooltip = "Status: Đang được sử dụng";
                    break;
                    
                case 3: // Reserved (Đã được đặt)
                    background = (SolidColorBrush)FindResource("ReservedTableColor");
                    tooltip = "Status: Đã được đặt";
                    break;
                    
                default:
                    background = Brushes.LightGray;
                    tooltip = "Status: Unknown";
                    break;
            }
            
            button.Background = background;
            button.Foreground = textColor;
            button.ToolTip = tooltip;
        }
        
        private void Table_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                string tableName = button.Content.ToString();
                
                if (_tableData.TryGetValue(tableName, out RestaurantTable table))
                {
                    // Store selected table
                    _selectedTable = table;
                    
                    // Update details in popup
                    TableDetailsHeader.Text = $"Table {tableName}";
                    TableStatusComboBox.SelectedIndex = table.TableStatusId - 1;
                    
                    // Hide status message and show popup
                    StatusMessageText.Visibility = Visibility.Collapsed;
                    TableDetailsPopup.Visibility = Visibility.Visible;
                }
            }
        }
        
        private void UpdateStatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTable != null && TableStatusComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                try
                {
                    int newStatusId = Convert.ToInt32(selectedItem.Tag);
                    
                    _selectedTable.TableStatusId = newStatusId;
                    
                    bool success = _tableService.UpdateRestaurantTable(_selectedTable);
                    
                    if (success)
                    {
                        // Update button appearance
                        string tableName = _selectedTable.TableName;
                        Button tableButton = FindName("Table" + tableName) as Button;
                        if (tableButton != null)
                        {
                            UpdateTableButtonAppearance(tableButton, _selectedTable);
                        }
                        
                        // Show success message
                        StatusMessageText.Text = $"Table {tableName} status updated successfully.";
                        StatusMessageText.Foreground = Brushes.Green;
                        StatusMessageText.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        throw new Exception("Failed to update table status in database.");
                    }
                }
                catch (Exception ex)
                {
                    // Show error message
                    StatusMessageText.Text = $"Error updating status: {ex.Message}";
                    StatusMessageText.Foreground = Brushes.Red;
                    StatusMessageText.Visibility = Visibility.Visible;
                }
            }
        }
        
        private void ClosePopupButton_Click(object sender, RoutedEventArgs e)
        {
            TableDetailsPopup.Visibility = Visibility.Collapsed;
        }
        
        private void BackToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Return to main menu
            var mainMenuWindow = new MainMenuWindow(_loggedInUser);
            mainMenuWindow.Show();
            this.Close();
        }
        
        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            // Reload tables from database
            LoadTables();
        }
    }
}