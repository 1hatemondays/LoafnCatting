using BusinessLayer.IService;
using BusinessLayer.Service;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        private ObservableCollection<Cat> _allCats;
        private ObservableCollection<Cat> _filteredCats;
        private Cat _selectedCat;
        private bool _isEditMode = false;
        private DispatcherTimer _timer;
        private string _selectedOriginalAbsoluteImagePath = null;
        private const string ImagesRelativeSaveBase = "Images/";
        private const string CatsSubfolderName = "Cats";

        public CatManagementWindow(User user)
        {
            InitializeComponent();
            _loggedInUser = user;

            _context = new LoafNcattingDbContext();
            _catService = new CatService(new CatRepository(_context));

            // Initialize collections
            _allCats = new ObservableCollection<Cat>();
            _filteredCats = new ObservableCollection<Cat>();

            SetupTimer();
            LoadInitialData();
            SetupDataGrid();
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

        private void SetupDataGrid()
        {
            CatsDataGrid.ItemsSource = _filteredCats;
        }

        private void LoadInitialData()
        {
            try
            {
                // Load cats
                var cats = _catService.GetAllCats();
                _allCats.Clear();
                foreach (var cat in cats)
                {
                    _allCats.Add(cat);
                }

                // Load genders and statuses
                GenderComboBox.ItemsSource = _context.Genders.ToList();
                StatusComboBox.ItemsSource = _context.CatStatuses.ToList();

                RefreshCatsList();
                UpdateCatCount();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error loading data: {ex.Message}");
            }
        }

        private void RefreshCatsList()
        {
            var searchText = SearchTextBox?.Text?.ToLower() ?? "";
            var selectedStatusId = StatusFilterComboBox?.SelectedValue != null
                ? (int)StatusFilterComboBox.SelectedValue
                : 0;

            _filteredCats.Clear();

            var filteredCats = _allCats.Where(c =>
                (string.IsNullOrEmpty(searchText) ||
                 c.Name.ToLower().Contains(searchText) ||
                 (c.Description?.ToLower().Contains(searchText) ?? false) ||
                 (c.Breed?.ToLower().Contains(searchText) ?? false)) &&
                (selectedStatusId == 0 || c.StatusId == selectedStatusId)
            );

            foreach (var cat in filteredCats)
            {
                _filteredCats.Add(cat);
            }

            UpdateCatCount();
        }

        private void UpdateCatCount()
        {
            CatCountTextBlock.Text = $"Total Cats: {_filteredCats.Count}";
        }

        #region Event Handlers

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshCatsList();
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshCatsList();
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            StatusFilterComboBox.SelectedIndex = 0;
            RefreshCatsList();
        }

        private void CatsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CatsDataGrid.SelectedItem is Cat selectedCat)
            {
                LoadCatToForm(selectedCat);
            }
        }

        private void EditCatButton_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is Cat cat)
            {
                LoadCatToForm(cat);
                _isEditMode = true;
                FormTitleTextBlock.Text = "Edit Cat";
                SaveButton.Content = "Update Cat";
            }
        }

        private void DeleteCatButton_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is Cat cat)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete '{cat.Name}'?",
                    "Delete Cat",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (_catService.DeleteCat(cat.CatId))
                        {
                            _allCats.Remove(cat);
                            RefreshCatsList();
                            ClearForm();
                            ShowSuccessMessage("Cat deleted successfully!");
                        }
                        else
                        {
                            ShowErrorMessage("Failed to delete cat.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowErrorMessage($"Error deleting cat: {ex.Message}");
                    }
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    var cat = CreateCatFromForm();

                    if (_isEditMode)
                    {
                        if (_catService.UpdateCat(cat))
                        {
                            // Update in collection
                            var existingCat = _allCats.FirstOrDefault(c => c.CatId == cat.CatId);
                            if (existingCat != null)
                            {
                                var index = _allCats.IndexOf(existingCat);
                                _allCats[index] = cat;
                            }
                            RefreshCatsList();
                            ShowSuccessMessage("Cat updated successfully!");
                            ClearForm();
                        }
                        else
                        {
                            ShowErrorMessage("Failed to update cat.");
                        }
                    }
                    else
                    {
                        if (_catService.AddCat(cat))
                        {
                            // Reload cats to get the new ID
                            LoadInitialData();
                            ShowSuccessMessage("Cat added successfully!");
                            ClearForm();
                        }
                        else
                        {
                            ShowErrorMessage("Failed to add cat.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage($"Error saving cat: {ex.Message}");
                }
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void NewCatButton_Click(object sender, RoutedEventArgs e)
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

        private void BrowsePictureButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select Cat Image",
                Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedOriginalAbsoluteImagePath = openFileDialog.FileName;
                PictureTextBox.Text = Path.Combine(ImagesRelativeSaveBase, CatsSubfolderName, Path.GetFileName(openFileDialog.FileName)).Replace("\\", "/");
            }
        }
        #endregion

        #region Helper Methods

        private void LoadCatToForm(Cat cat)
        {
            _selectedCat = cat;
            NameTextBox.Text = cat.Name;
            AgeTextBox.Text = cat.Age?.ToString() ?? "";
            BreedTextBox.Text = cat.Breed ?? "";
            FriendlinessRatingTextBox.Text = cat.FriendlinessRating?.ToString() ?? "";
            DescriptionTextBox.Text = cat.Description ?? "";
            GenderComboBox.SelectedValue = cat.GenderId;
            StatusComboBox.SelectedValue = cat.StatusId;
            PictureTextBox.Text = cat.Picture ?? "";
            _selectedOriginalAbsoluteImagePath = null;

            _isEditMode = true;
            FormTitleTextBlock.Text = "Edit Cat";
            SaveButton.Content = "Update Cat";
        }

        private Cat CreateCatFromForm()
        {
            var cat = new Cat
            {
                Name = NameTextBox.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text.Trim(),
                Age = int.TryParse(AgeTextBox.Text, out var age) ? age : (int?)null,
                Breed = string.IsNullOrWhiteSpace(BreedTextBox.Text) ? null : BreedTextBox.Text.Trim(),
                FriendlinessRating = int.TryParse(FriendlinessRatingTextBox.Text, out var rating) ? rating : (int?)null,
                GenderId = (int)GenderComboBox.SelectedValue,
                StatusId = (int)StatusComboBox.SelectedValue,
                Picture = string.IsNullOrWhiteSpace(PictureTextBox.Text) ? null : PictureTextBox.Text.Trim()
            };

            if (_isEditMode && _selectedCat != null)
            {
                cat.CatId = _selectedCat.CatId;
            }

            if (_selectedOriginalAbsoluteImagePath != null && File.Exists(_selectedOriginalAbsoluteImagePath))
            {
                try
                {
                    string sourceFileName = Path.GetFileName(_selectedOriginalAbsoluteImagePath);
                    string destinationDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ImagesRelativeSaveBase, CatsSubfolderName);
                    string destinationFullPath = Path.Combine(destinationDirectory, sourceFileName);

                    if (!Directory.Exists(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }

                    File.Copy(_selectedOriginalAbsoluteImagePath, destinationFullPath, true);
                    System.Diagnostics.Debug.WriteLine($"Image copied from '{_selectedOriginalAbsoluteImagePath}' to '{destinationFullPath}'");

                    cat.Picture = Path.Combine(ImagesRelativeSaveBase, CatsSubfolderName, sourceFileName).Replace("\\", "/");
                }
                catch (Exception ex)
                {
                    ShowErrorMessage($"Failed to save image file: {ex.Message}");
                    cat.Picture = _selectedCat?.Picture;
                }
            }
            else if (_isEditMode && _selectedCat != null && _selectedOriginalAbsoluteImagePath == null)
            {
                cat.Picture = PictureTextBox.Text.Trim();
            }
            else
            {
                cat.Picture = null;
            }

            return cat;
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

            if (!string.IsNullOrWhiteSpace(AgeTextBox.Text) && !int.TryParse(AgeTextBox.Text, out var age))
                errors.Add("Age must be a valid number.");

            if (!string.IsNullOrWhiteSpace(FriendlinessRatingTextBox.Text))
            {
                if (!int.TryParse(FriendlinessRatingTextBox.Text, out var rating))
                    errors.Add("Friendliness rating must be a number.");
                else if (rating < 1 || rating > 5)
                    errors.Add("Friendliness rating must be between 1 and 5.");
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
            _selectedCat = null;
            _isEditMode = false;
            FormTitleTextBlock.Text = "Add New Cat";
            SaveButton.Content = "Save Cat";

            NameTextBox.Text = "";
            AgeTextBox.Text = "";
            BreedTextBox.Text = "";
            FriendlinessRatingTextBox.Text = "";
            DescriptionTextBox.Text = "";
            GenderComboBox.SelectedIndex = -1;
            StatusComboBox.SelectedIndex = -1;
            PictureTextBox.Text = "";
            _selectedOriginalAbsoluteImagePath = null;
            ValidationMessageTextBlock.Visibility = Visibility.Collapsed;
            CatsDataGrid.SelectedItem = null;
        }

        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer?.Stop();
            base.OnClosed(e);
        }

        #endregion
    }
}