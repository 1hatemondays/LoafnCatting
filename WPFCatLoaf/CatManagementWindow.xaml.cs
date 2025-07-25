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
using System.Windows.Threading;

namespace WPFCatLoaf
{
    public partial class CatManagementWindow : Window
    {
        private readonly ICatService _catService;
        private readonly LoafNcattingDbContext _context;
        private readonly User _loggedInUser;
        private ObservableCollection<Cat> _cats;
        private Cat _selectedCat;
        private bool _isEditMode = false;
        private DispatcherTimer _timer;

        public CatManagementWindow(User user)
        {
            InitializeComponent();
            _loggedInUser = user;

            _context = new LoafNcattingDbContext();
            _catService = new CatService(new CatRepository(_context));

            SetupTimer();
            LoadInitialData();
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
                _cats = new ObservableCollection<Cat>(_catService.GetAllCats());
                CatsDataGrid.ItemsSource = _cats;

                // Load genders and statuses directly from context
                GenderComboBox.ItemsSource = _context.Genders.ToList();
                StatusComboBox.ItemsSource = _context.CatStatuses.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CatsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CatsDataGrid.SelectedItem is Cat selectedCat)
            {
                _selectedCat = selectedCat;
                _isEditMode = true;
                FormTitleTextBlock.Text = "Edit Cat";
                LoadCatToForm(selectedCat);
            }
        }

        private void LoadCatToForm(Cat cat)
        {
            NameTextBox.Text = cat.Name;
            AgeTextBox.Text = cat.Age?.ToString();
            BreedTextBox.Text = cat.Breed;
            FriendlinessRatingTextBox.Text = cat.FriendlinessRating?.ToString();
            DescriptionTextBox.Text = cat.Description;
            GenderComboBox.SelectedValue = cat.GenderId;
            StatusComboBox.SelectedValue = cat.StatusId;
            PictureTextBox.Text = cat.Picture;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            var cat = CreateCatFromForm();

            try
            {
                if (_isEditMode)
                {
                    cat.CatId = _selectedCat.CatId;
                    if (_catService.UpdateCat(cat))
                    {
                        var index = _cats.IndexOf(_selectedCat);
                        _cats[index] = cat;
                        MessageBox.Show("Cat updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearForm();
                        LoadInitialData(); // Refresh the data
                    }
                    else
                    {
                        MessageBox.Show("Failed to update cat.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else // Add new cat
                {
                    if (_catService.AddCat(cat))
                    {
                        _cats.Add(cat);
                        MessageBox.Show("Cat added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearForm();
                        LoadInitialData(); // Refresh the data
                    }
                    else
                    {
                        MessageBox.Show("Failed to add cat.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Cat CreateCatFromForm()
        {
            return new Cat
            {
                Name = NameTextBox.Text,
                Age = int.TryParse(AgeTextBox.Text, out var age) ? age : (int?)null,
                Breed = BreedTextBox.Text,
                FriendlinessRating = int.TryParse(FriendlinessRatingTextBox.Text, out var rating) ? rating : (int?)null,
                Description = DescriptionTextBox.Text,
                GenderId = (int)GenderComboBox.SelectedValue,
                StatusId = (int)StatusComboBox.SelectedValue,
                Picture = PictureTextBox.Text
            };
        }

        private void ClearForm()
        {
            _selectedCat = null;
            _isEditMode = false;
            FormTitleTextBlock.Text = "Add New Cat";
            NameTextBox.Clear();
            AgeTextBox.Clear();
            BreedTextBox.Clear();
            FriendlinessRatingTextBox.Clear();
            DescriptionTextBox.Clear();
            GenderComboBox.SelectedIndex = -1;
            StatusComboBox.SelectedIndex = -1;
            PictureTextBox.Clear();
            CatsDataGrid.SelectedItem = null;
            ValidationMessageTextBlock.Visibility = Visibility.Collapsed;
        }

        private bool ValidateForm()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                errors.Add("Cat name is required.");
            if (GenderComboBox.SelectedValue == null)
                errors.Add("Gender is required.");
            if (StatusComboBox.SelectedValue == null)
                errors.Add("Status is required.");
            if (!string.IsNullOrWhiteSpace(AgeTextBox.Text) && !int.TryParse(AgeTextBox.Text, out _))
                errors.Add("Age must be a valid number.");
            if (!string.IsNullOrWhiteSpace(FriendlinessRatingTextBox.Text) && (!int.TryParse(FriendlinessRatingTextBox.Text, out int rating) || rating < 1 || rating > 5))
                errors.Add("Friendliness rating must be between 1 and 5.");

            if (errors.Any())
            {
                ValidationMessageTextBlock.Text = string.Join("\n", errors);
                ValidationMessageTextBlock.Visibility = Visibility.Visible;
                return false;
            }

            ValidationMessageTextBlock.Visibility = Visibility.Collapsed;
            return true;
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

        protected override void OnClosed(EventArgs e)
        {
            _timer?.Stop();
            base.OnClosed(e);
        }
    }
}