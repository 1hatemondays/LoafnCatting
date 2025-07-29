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
using System.Windows.Media.Imaging;

namespace WPFCatLoaf
{
    public partial class ProductManagementWindow : Window
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly User _loggedInUser;
        private ObservableCollection<Product> _allProducts;
        private ObservableCollection<Product> _filteredProducts;
        private Product _selectedProduct;
        private bool _isEditMode = false;
        private string _selectedOriginalAbsoluteImagePath = null;
        private const string ImagesRelativeSaveBase = "Images/";
        private const string ProductsSubfolderName = "Products";

        public ProductManagementWindow(User user)
        {
            InitializeComponent();
            _loggedInUser = user;

            // Set welcome message
            WelcomeTextBlock.Text = $"Welcome, {user.Name}! Manage your cafe products with ease";

            // Initialize services
            var context = new LoafNcattingDbContext();
            _productService = new ProductService(new ProductRepository(context));
            _categoryService = new CategoryService(new CategoryRepository(context));

            // Initialize collections
            _allProducts = new ObservableCollection<Product>();
            _filteredProducts = new ObservableCollection<Product>();

            LoadInitialData();
            SetupDataGrid();
        }

        private void LoadInitialData()
        {
            try
            {
                // Load products
                var products = _productService.GetAllProducts();
                _allProducts.Clear();
                foreach (var product in products)
                {
                    _allProducts.Add(product);
                }

                // Load categories
                var categories = _categoryService.GetAllCategories();
                CategoryComboBox.ItemsSource = categories;

                // Add "All Categories" option for filter
                var filterCategories = new List<Category>
                {
                    new Category { CategoryId = 0, Name = "All Categories" }
                };
                filterCategories.AddRange(categories);
                CategoryFilterComboBox.ItemsSource = filterCategories;
                CategoryFilterComboBox.SelectedIndex = 0;

                RefreshProductsList();
                UpdateProductCount();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error loading data: {ex.Message}");
            }
        }

        private void SetupDataGrid()
        {
            ProductsDataGrid.ItemsSource = _filteredProducts;
        }

        private void RefreshProductsList()
        {
            var searchText = SearchTextBox.Text?.ToLower() ?? "";
            var selectedCategoryId = CategoryFilterComboBox.SelectedValue != null
                ? (int)CategoryFilterComboBox.SelectedValue
                : 0;

            _filteredProducts.Clear();

            var filteredProducts = _allProducts.Where(p =>
                (string.IsNullOrEmpty(searchText) ||
                 p.Name.ToLower().Contains(searchText) ||
                 (p.Description?.ToLower().Contains(searchText) ?? false)) &&
                (selectedCategoryId == 0 || p.CategoryId == selectedCategoryId)
            );

            foreach (var product in filteredProducts)
            {
                _filteredProducts.Add(product);
            }

            UpdateProductCount();
        }

        private void UpdateProductCount()
        {
            ProductCountTextBlock.Text = $"Total Products: {_filteredProducts.Count}";
        }

        #region Event Handlers

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshProductsList();
        }

        private void CategoryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshProductsList();
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            CategoryFilterComboBox.SelectedIndex = 0;
            RefreshProductsList();
        }

        private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is Product selectedProduct)
            {
                _isEditMode = true;
                LoadProductToForm(selectedProduct);
                FormTitleTextBlock.Text = "Edit Product";
                SaveButton.Content = "Update Product";
            }
        }
        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is Product product)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete '{product.Name}'?",
                    "Delete Product",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (_productService.DeleteProduct(product.ProductId))
                        {
                            _allProducts.Remove(product);
                            RefreshProductsList();
                            ClearForm();
                            ShowSuccessMessage("Product deleted successfully!");
                        }
                        else
                        {
                            ShowErrorMessage("Failed to delete product.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowErrorMessage($"Error deleting product: {ex.Message}");
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
                    var product = CreateProductFromForm();

                    if (_isEditMode)
                    {
                        if (_productService.UpdateProduct(product))
                        {
                            LoadInitialData();
                            ShowSuccessMessage("Product updated successfully!");
                            ClearForm();
                        }
                        else
                        {
                            ShowErrorMessage("Failed to update product.");
                        }
                    }
                    else
                    {
                        if (_productService.AddProduct(product))
                        {
                            // Reload products to get the new ID
                            LoadInitialData();
                            ShowSuccessMessage("Product added successfully!");
                            ClearForm();
                        }
                        else
                        {
                            ShowErrorMessage("Failed to add product.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage($"Error saving product: {ex.Message}");
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void NewProductButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void BackToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Allow both admin and staff to access main menu


            var mainMenuWindow = new MainMenuWindow(_loggedInUser);
            mainMenuWindow.Show();
            this.Close();


        }


        //private void PictureTextBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    UpdateImagePreview();
        //}

        private void BrowsePictureButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select Product Image",
                Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedOriginalAbsoluteImagePath = openFileDialog.FileName;
                PictureTextBox.Text = Path.Combine(ImagesRelativeSaveBase, ProductsSubfolderName, Path.GetFileName(openFileDialog.FileName)).Replace("\\", "/");
            }
        }

        #endregion

        #region Helper Methods

        //private void UpdateImagePreview()
        //{
        //    try
        //    {
        //        if (!string.IsNullOrWhiteSpace(PictureTextBox.Text))
        //        {
        //            if (Uri.IsWellFormedUriString(PictureTextBox.Text, UriKind.Absolute) || File.Exists(PictureTextBox.Text))
        //            {
        //                var bitmap = new BitmapImage();
        //                bitmap.BeginInit();
        //                bitmap.UriSource = new Uri(PictureTextBox.Text, UriKind.RelativeOrAbsolute);
        //                bitmap.CacheOption = BitmapCacheOption.OnLoad;
        //                bitmap.EndInit();
        //                ImagePreview.Source = bitmap;
        //                ImagePreview.Visibility = Visibility.Visible;
        //                return;
        //            }
        //        }

        //        // Show placeholder if no valid image
        //        ImagePreview.Source = null;
        //        ImagePreview.Visibility = Visibility.Collapsed;
        //    }
        //    catch
        //    {
        //        // Show placeholder on error
        //        ImagePreview.Source = null;
        //        ImagePreview.Visibility = Visibility.Collapsed;
        //    }
        //}

        private void LoadProductToForm(Product product)
        {
            _selectedProduct = product;
            ProductNameTextBox.Text = product.Name;
            DescriptionTextBox.Text = product.Description ?? "";
            PriceTextBox.Text = product.Price.ToString("F2");
            UnitInStockTextBox.Text = product.UnitInStock.ToString();
            CategoryComboBox.SelectedValue = product.CategoryId;
            PictureTextBox.Text = product.Picture ?? "";
            _selectedOriginalAbsoluteImagePath = null;
        }

        private Product CreateProductFromForm()
        {
            var product = new Product
            {
                Name = ProductNameTextBox.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text.Trim(),
                Price = decimal.Parse(PriceTextBox.Text),
                UnitInStock = int.Parse(UnitInStockTextBox.Text),
                CategoryId = (int)CategoryComboBox.SelectedValue,
                Picture = string.IsNullOrWhiteSpace(PictureTextBox.Text) ? null : PictureTextBox.Text.Trim()
            };

            if (_isEditMode && _selectedProduct != null)
            {
                product.ProductId = _selectedProduct.ProductId;
            }
            if (_selectedOriginalAbsoluteImagePath != null && File.Exists(_selectedOriginalAbsoluteImagePath))
            {
                try
                {
                    string sourceFileName = Path.GetFileName(_selectedOriginalAbsoluteImagePath);
                    string destinationDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ImagesRelativeSaveBase, ProductsSubfolderName);
                    string destinationFullPath = Path.Combine(destinationDirectory, sourceFileName);

                    if (!Directory.Exists(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }

                    File.Copy(_selectedOriginalAbsoluteImagePath, destinationFullPath, true);
                    System.Diagnostics.Debug.WriteLine($"Image copied from '{_selectedOriginalAbsoluteImagePath}' to '{destinationFullPath}'");

                    product.Picture = Path.Combine(ImagesRelativeSaveBase, ProductsSubfolderName, sourceFileName).Replace("\\", "/");
                }
                catch (Exception ex)
                {
                    ShowErrorMessage($"Failed to save image file: {ex.Message}");
                    product.Picture = _selectedProduct?.Picture;
                }
            }
            else if (_isEditMode && _selectedProduct != null && _selectedOriginalAbsoluteImagePath == null)
            {
                product.Picture = PictureTextBox.Text.Trim();
            }
            else
            {
                product.Picture = null;
            }
            return product;
        }

        private bool ValidateForm()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text))
                errors.Add("Product name is required.");

            if (!decimal.TryParse(PriceTextBox.Text, out var price) || price <= 0)
                errors.Add("Valid price is required.");

            if (!int.TryParse(UnitInStockTextBox.Text, out var stock) || stock < 0)
                errors.Add("Valid units in stock is required.");

            if (CategoryComboBox.SelectedValue == null)
                errors.Add("Category is required.");

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
            _selectedProduct = null;
            _isEditMode = false;
            FormTitleTextBlock.Text = "Add New Product";
            SaveButton.Content = "Save Product";

            ProductNameTextBox.Text = "";
            DescriptionTextBox.Text = "";
            PriceTextBox.Text = "";
            UnitInStockTextBox.Text = "";
            CategoryComboBox.SelectedIndex = -1;
            PictureTextBox.Text = "";
            _selectedOriginalAbsoluteImagePath = null;
            ValidationMessageTextBlock.Visibility = Visibility.Collapsed;
            ProductsDataGrid.SelectedItem = null;
        }

        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion
    }
}