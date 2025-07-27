using BusinessLayer.IService;
using BusinessLayer.Service;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WPFCatLoaf
{
    public partial class ReservationManagementWindow : Window
    {
        private readonly IReservationService _reservationService;
        private readonly IRestaurantTableService _tableService;
        private readonly LoafNcattingDbContext _context;
        private readonly User _loggedInUser;
        private DispatcherTimer _timer;
        private Reservation _selectedReservation;
        private bool _isEditMode = false;

        public ReservationManagementWindow(User user)
        {
            InitializeComponent();
            _loggedInUser = user;
            _context = new LoafNcattingDbContext();
            _reservationService = new ReservationService(new ReservationRepository(_context));
            _tableService = new RestaurantTableService(new RestaurantTableRepository(_context));

            SetupDateTimeConstraints();
            SetupTimer();
            LoadInitialData();
        }

        private void SetupDateTimeConstraints()
        {
            // Set minimum date to today to prevent past date selection
            ReservationDatePicker.DisplayDateStart = DateTime.Today;
            ReservationDatePicker.BlackoutDates.AddDatesInPast();
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

        private void LoadInitialData()
        {
            try
            {
                // Load reservations
                ReservationsDataGrid.ItemsSource = _reservationService.GetAllReservation();

                // Load only available tables (TableStatusId = 1) for reservation
                var availableTables = _tableService.GetRestaurantTableByStatusId(1); // Only available tables
                TableComboBox.ItemsSource = availableTables;

                // Load all reservation statuses for ComboBox (only used in edit mode)
                var reservationStatuses = _context.ReservationStatuses.ToList();
                StatusComboBox.ItemsSource = reservationStatuses;
                
                // Setup form for Add mode initially
                SetupFormForAddMode();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupFormForAddMode()
        {
            // Hide status controls for Add mode (status will be automatically set to 1)
            StatusLabel.Visibility = Visibility.Collapsed;
            StatusComboBox.Visibility = Visibility.Collapsed;
            
            _isEditMode = false;
            FormTitleTextBlock.Text = "Add New Reservation";
        }

        private void SetupFormForEditMode()
        {
            // Show status controls for Edit mode (allow changing status)
            StatusLabel.Visibility = Visibility.Visible;
            StatusComboBox.Visibility = Visibility.Visible;
            
            _isEditMode = true;
            FormTitleTextBlock.Text = "Edit Reservation";
        }

        private void ReservationsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReservationsDataGrid.SelectedItem is Reservation selectedReservation)
            {
                _selectedReservation = selectedReservation;
                SetupFormForEditMode();
                LoadReservationToForm(selectedReservation);
            }
        }

        private void LoadReservationToForm(Reservation reservation)
        {
            GuestNameTextBox.Text = reservation.GuestName;
            GuestPhoneNumberTextBox.Text = reservation.GuestPhoneNumber;
            ReservationDatePicker.SelectedDate = reservation.Date.ToDateTime(TimeOnly.MinValue);
            ReservationTimeTextBox.Text = reservation.Time.ToString("HH:mm");
            TableComboBox.SelectedValue = reservation.TableId;
            StatusComboBox.SelectedValue = reservation.StatusId;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                var reservation = CreateReservationFromForm();

                if (_isEditMode)
                {
                    reservation.ReservationId = _selectedReservation.ReservationId;
                    if (_reservationService.UpdateReservation(reservation))
                    {
                        MessageBox.Show("Reservation updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearForm();
                        LoadInitialData(); // Refresh data
                    }
                    else
                    {
                        MessageBox.Show("Failed to update reservation.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else // Add new reservation
                {
                    if (_reservationService.AddReservation(reservation))
                    {
                        // Update table status to 3 (Reserved/Occupied) after successful reservation
                        UpdateTableStatusAfterReservation(reservation.TableId);
                        
                        MessageBox.Show("Reservation added successfully! Table status updated to reserved.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearForm();
                        LoadInitialData(); // Refresh data
                    }
                    else
                    {
                        MessageBox.Show("Failed to add reservation.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving reservation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTableStatusAfterReservation(int tableId)
        {
            try
            {
                // Get the table and update its status to 3 (Reserved/Occupied)
                var table = _tableService.GetRestaurantTableById(tableId);
                if (table != null)
                {
                    table.TableStatusId = 3; // 3 = Reserved/Occupied status
                    bool updateResult = _tableService.UpdateRestaurantTable(table);
                    
                    if (!updateResult)
                    {
                        // Log warning but don't fail the reservation
                        MessageBox.Show("Reservation saved but failed to update table status.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the reservation
                MessageBox.Show($"Reservation saved but error updating table status: {ex.Message}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private Reservation CreateReservationFromForm()
        {
            return new Reservation
            {
                GuestName = GuestNameTextBox.Text.Trim(),
                GuestPhoneNumber = GuestPhoneNumberTextBox.Text.Trim(),
                Date = DateOnly.FromDateTime(ReservationDatePicker.SelectedDate.Value),
                Time = TimeOnly.Parse(ReservationTimeTextBox.Text.Trim()),
                TableId = (int)TableComboBox.SelectedValue,
                StatusId = _isEditMode ? (int)StatusComboBox.SelectedValue : 1 // Always 1 for new reservations, use ComboBox value for edit
            };
        }

        private bool ValidateForm()
        {
            var errors = new List<string>();

            // Basic required field validation
            if (string.IsNullOrWhiteSpace(GuestNameTextBox.Text))
                errors.Add("Guest name is required.");
            if (string.IsNullOrWhiteSpace(GuestPhoneNumberTextBox.Text))
                errors.Add("Phone number is required.");
            if (ReservationDatePicker.SelectedDate == null)
                errors.Add("Date is required.");
            if (string.IsNullOrWhiteSpace(ReservationTimeTextBox.Text))
                errors.Add("Time is required.");
            if (TableComboBox.SelectedValue == null)
                errors.Add("Table selection is required.");
            
            // Status validation only for edit mode
            if (_isEditMode && StatusComboBox.SelectedValue == null)
                errors.Add("Status selection is required.");

            // Table availability validation - only for new reservations
            if (!_isEditMode && TableComboBox.SelectedValue != null)
            {
                var selectedTableId = (int)TableComboBox.SelectedValue;
                var selectedTable = _tableService.GetRestaurantTableById(selectedTableId);
                
                if (selectedTable != null && selectedTable.TableStatusId != 1)
                {
                    errors.Add("Selected table is not available for reservation. Please choose an available table.");
                }
            }

            // Date validation
            if (ReservationDatePicker.SelectedDate.HasValue)
            {
                var selectedDate = ReservationDatePicker.SelectedDate.Value.Date;
                var today = DateTime.Today;
                
                if (selectedDate < today)
                {
                    errors.Add("Cannot select a date in the past.");
                }
            }

            // Time validation
            if (!string.IsNullOrWhiteSpace(ReservationTimeTextBox.Text))
            {
                if (TimeOnly.TryParse(ReservationTimeTextBox.Text.Trim(), out var selectedTime))
                {
                    var openingTime = new TimeOnly(8, 0); // 8:00 AM
                    var closingTime = new TimeOnly(22, 0); // 10:00 PM
                    
                    if (selectedTime < openingTime || selectedTime > closingTime)
                    {
                        errors.Add("Reservation time must be between 8:00 AM and 10:00 PM.");
                    }

                    // Additional validation: if date is today, time cannot be in the past
                    if (ReservationDatePicker.SelectedDate.HasValue)
                    {
                        var selectedDate = ReservationDatePicker.SelectedDate.Value.Date;
                        var today = DateTime.Today;
                        
                        if (selectedDate == today)
                        {
                            var currentTime = TimeOnly.FromDateTime(DateTime.Now);
                            if (selectedTime <= currentTime)
                            {
                                errors.Add("Cannot select a time in the past for today's date.");
                            }
                        }
                    }
                }
                else
                {
                    errors.Add("Invalid time format. Use HH:mm (e.g., 14:30).");
                }
            }

            if (errors.Any())
            {
                ValidationMessageTextBlock.Text = string.Join("\n", errors);
                ValidationMessageTextBlock.Visibility = Visibility.Visible;
                return false;
            }

            ValidationMessageTextBlock.Visibility = Visibility.Collapsed;
            return true;
        }

        private void ClearForm()
        {
            _selectedReservation = null;
            
            GuestNameTextBox.Clear();
            GuestPhoneNumberTextBox.Clear();
            ReservationDatePicker.SelectedDate = null;
            ReservationTimeTextBox.Clear();
            TableComboBox.SelectedIndex = -1;
            StatusComboBox.SelectedIndex = -1;
            ReservationsDataGrid.SelectedItem = null;
            ValidationMessageTextBlock.Visibility = Visibility.Collapsed;
            
            // Setup form for Add mode (this will hide status controls and set proper title)
            SetupFormForAddMode();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void BackToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            _timer?.Stop();
            var mainMenuWindow = new MainMenuWindow(_loggedInUser);
            mainMenuWindow.Show();
            this.Close();
        }

        private void ReservationTimeTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateTimeInput();
        }

        private void ValidateTimeInput()
        {
            var timeText = ReservationTimeTextBox.Text.Trim();
            
            if (string.IsNullOrEmpty(timeText))
            {
                ReservationTimeTextBox.BorderBrush = System.Windows.Media.Brushes.LightGray;
                return;
            }

            if (TimeOnly.TryParse(timeText, out var selectedTime))
            {
                var openingTime = new TimeOnly(8, 0); // 8:00 AM
                var closingTime = new TimeOnly(22, 0); // 10:00 PM
                
                if (selectedTime >= openingTime && selectedTime <= closingTime)
                {
                    // Valid time - green border
                    ReservationTimeTextBox.BorderBrush = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    // Invalid time range - red border
                    ReservationTimeTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
                }
            }
            else
            {
                // Invalid format - red border
                ReservationTimeTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer?.Stop();
            base.OnClosed(e);
        }
    }
}