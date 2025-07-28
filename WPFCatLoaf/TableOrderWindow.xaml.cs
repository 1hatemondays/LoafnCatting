using BusinessLayer.IService;
using BusinessLayer.Service;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFCatLoaf.Converters;

namespace WPFCatLoaf
{
    public partial class TableOrderWindow : Window
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IRestaurantTableService _tableService;
        private readonly IPaymentService _paymentService;

        private readonly User _loggedInUser;
        private RestaurantTable _currentTable;
        private ObservableCollection<CartItem> _cartItems;
        private List<Product> _allProducts;
        private List<Category> _allCategories;
        private int? _selectedPaymentMethodId = null;

        private readonly ImagePathConverter _imagePathConverter = new ImagePathConverter();
        // Constructor for staff/table users
        public TableOrderWindow(User user)
        {
            InitializeComponent();
            _loggedInUser = user;

            // Initialize services
            var context = new LoafNcattingDbContext();
            _productService = new ProductService(new ProductRepository(context));
            _categoryService = new CategoryService(new CategoryRepository(context));
            _orderService = new OrderService(new OrderRepository(context));
            _orderDetailService = new OrderDetailService(new OrderDetailRepository(context));
            _tableService = new RestaurantTableService(new RestaurantTableRepository(context));
            _paymentService = new PaymentService(new PaymentRepository(context));

            InitializeForLoggedInUser();
        }

        // Constructor for customers with table ID (new)
        public TableOrderWindow(int tableId)
        {
            InitializeComponent();
            _loggedInUser = null; // No user for customers

            // Initialize services
            var context = new LoafNcattingDbContext();
            _productService = new ProductService(new ProductRepository(context));
            _categoryService = new CategoryService(new CategoryRepository(context));
            _orderService = new OrderService(new OrderRepository(context));
            _orderDetailService = new OrderDetailService(new OrderDetailRepository(context));
            _tableService = new RestaurantTableService(new RestaurantTableRepository(context));
            _paymentService = new PaymentService(new PaymentRepository(context));

            InitializeForCustomer(tableId);
        }

        private void InitializeCommonComponents()
        {
            // Initialize cart
            _cartItems = new ObservableCollection<CartItem>();
            CartItemsControl.ItemsSource = _cartItems;

            // Subscribe to cart changes
            _cartItems.CollectionChanged += CartItems_CollectionChanged;
        }

        private void InitializeForLoggedInUser()
        {
            InitializeCommonComponents();

            try
            {
                // Find the table associated with this user
                var tables = _tableService.GetAllRestaurantTables();
                _currentTable = tables.FirstOrDefault();

                LoadDataAndUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeForCustomer(int tableId)
        {
            InitializeCommonComponents();

            try
            {
                // Find the specific table
                var tables = _tableService.GetAllRestaurantTables();
                _currentTable = tables.FirstOrDefault(t => t.TableId == tableId);

                if (_currentTable == null)
                {
                    MessageBox.Show($"Table {tableId} not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                LoadDataAndUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading table data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void LoadDataAndUI()
        {
            // Load products and categories
            _allProducts = _productService.GetAllProducts();
            _allCategories = _categoryService.GetAllCategories();

            // Update UI
            if (_currentTable != null)
            {
                if (_loggedInUser != null)
                {
                    WelcomeTextBlock.Text = $"Welcome to CatLoaf Cafe!";
                    TableInfoTextBlock.Text = $"Staff: {_loggedInUser.Name} - Table: {_currentTable.TableName}";
                }
                else
                {
                    WelcomeTextBlock.Text = $"Welcome to CatLoaf Cafe!";
                    TableInfoTextBlock.Text = $"Table: {_currentTable.TableName} - Select your favorite items!";
                }
            }

            LoadCategoriesAndProducts();
        }

        private void LoadCategoriesAndProducts()
        {
            CategoriesPanel.Children.Clear();

            foreach (var category in _allCategories)
            {
                var categoryProducts = _allProducts.Where(p => p.CategoryId == category.CategoryId).ToList();
                if (!categoryProducts.Any()) continue;

                // Category Header
                var categoryHeader = new TextBlock
                {
                    Text = category.Name,
                    Style = (Style)FindResource("CategoryHeaderStyle")
                };
                CategoriesPanel.Children.Add(categoryHeader);

                // Category Description
                if (!string.IsNullOrEmpty(category.Description))
                {
                    var categoryDesc = new TextBlock
                    {
                        Text = category.Description,
                        FontSize = 14,
                        Foreground = Brushes.Gray,
                        Margin = new Thickness(0, 0, 0, 15)
                    };
                    CategoriesPanel.Children.Add(categoryDesc);
                }

                // Products Grid - Changed to 5 columns for better layout like the reference image
                var productsGrid = new UniformGrid
                {
                    Columns = 5, // 5 columns for product cards like the reference
                    Margin = new Thickness(0, 0, 0, 30)
                };

                foreach (var product in categoryProducts)
                {
                    var productCard = CreateProductCard(product);
                    productsGrid.Children.Add(productCard);
                }

                CategoriesPanel.Children.Add(productsGrid);
            }
        }

        private Border CreateProductCard(Product product)
        {
            var card = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(12),
                Margin = new Thickness(8),
                Padding = new Thickness(0),
                Width = 220,
                Height = 280,
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gray,
                    Direction = 315,
                    ShadowDepth = 2,
                    BlurRadius = 8,
                    Opacity = 0.3
                }
            };

            var mainGrid = new Grid();

            // Product Image (takes most of the space like reference)
            var imageContainer = new Border
            {
                CornerRadius = new CornerRadius(12, 12, 0, 0),
                Height = 160,
                VerticalAlignment = VerticalAlignment.Top,
                Background = new SolidColorBrush(Color.FromRgb(245, 245, 245))
            };

            var productImage = new Image
            {
                Stretch = Stretch.UniformToFill,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            if (!string.IsNullOrEmpty(product.Picture))
            {
                try
                {
                    productImage.Source = _imagePathConverter.Convert(product.Picture, typeof(ImageSource), null, CultureInfo.CurrentCulture) as ImageSource;
                }
                catch
                {
                    // Use placeholder if image fails to load
                    productImage.Source = null;
                }
            }

            imageContainer.Child = productImage;
            mainGrid.Children.Add(imageContainer);

            // Content area at the bottom
            var contentStack = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(12, 0, 12, 12)
            };

            // Brand/Category label (like "HIGHLANDS COFFEE" in reference)
            var brandLabel = new TextBlock
            {
                Text = _allCategories.FirstOrDefault(c => c.CategoryId == product.CategoryId)?.Name?.ToUpper() ?? "PRODUCT",
                FontSize = 10,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150)),
                Margin = new Thickness(0, 8, 0, 4)
            };
            contentStack.Children.Add(brandLabel);

            // Product Name
            var nameTextBlock = new TextBlock
            {
                Text = product.Name,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(51, 51, 51)),
                TextWrapping = TextWrapping.Wrap,
                MaxHeight = 36,
                Margin = new Thickness(0, 0, 0, 8)
            };
            contentStack.Children.Add(nameTextBlock);

            // Price and Add Button Row
            var bottomGrid = new Grid
            {
                Margin = new Thickness(0, 4, 0, 0)
            };
            bottomGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            bottomGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Price
            var priceTextBlock = new TextBlock
            {
                Text = $"{product.Price:N0} VND",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 107, 53)), // Orange color like reference
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(priceTextBlock, 0);
            bottomGrid.Children.Add(priceTextBlock);

            // Add Button (Plus icon in circle like reference) - Use round button style
            var addButton = new Button
            {
                Content = "+",
                Width = 32,
                Height = 32,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Tag = product,
                Style = (Style)FindResource("RoundAddButtonStyle")
            };

            addButton.Click += AddToCartButton_Click;
            Grid.SetColumn(addButton, 1);
            bottomGrid.Children.Add(addButton);

            contentStack.Children.Add(bottomGrid);
            mainGrid.Children.Add(contentStack);

            card.Child = mainGrid;
            return card;
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Product product)
            {
                var existingItem = _cartItems.FirstOrDefault(item => item.Product.ProductId == product.ProductId);

                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    _cartItems.Add(new CartItem { Product = product, Quantity = 1 });
                }

                UpdateCartDisplay();
            }
        }

        private void CartItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateCartDisplay();
        }

        private void UpdateCartDisplay()
        {
            var totalItems = _cartItems.Sum(item => item.Quantity);
            var totalPrice = _cartItems.Sum(item => item.Product.Price * item.Quantity);

            CartCountTextBlock.Text = totalItems.ToString();
            TotalPriceTextBlock.Text = $"{totalPrice:N0} VND";
            CartTotalTextBlock.Text = $"{totalPrice:N0} VND";
        }

        private void ViewCartButton_Click(object sender, RoutedEventArgs e)
        {
            CartOverlay.Visibility = Visibility.Visible;
        }

        private void CloseCartButton_Click(object sender, RoutedEventArgs e)
        {
            CartOverlay.Visibility = Visibility.Collapsed;
        }

        private void ContinueShoppingButton_Click(object sender, RoutedEventArgs e)
        {
            CartOverlay.Visibility = Visibility.Collapsed;
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is CartItem cartItem)
            {
                cartItem.Quantity++;
                UpdateCartDisplay();
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is CartItem cartItem)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                }
                else
                {
                    _cartItems.Remove(cartItem);
                }
                UpdateCartDisplay();
            }
        }

        private void ClearCartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_cartItems.Any())
            {
                MessageBox.Show("Your cart is already empty.",
                    "Cart Empty", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show("Are you sure you want to clear your cart?",
                "Clear Cart", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _cartItems.Clear();
                UpdateCartDisplay();
                ResetPaymentSelection();

                // Show confirmation
                MessageBox.Show("Cart cleared successfully!",
                    "Cart Cleared", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            PlaceOrder();
        }

        private void PlaceOrderButton_Click(object sender, RoutedEventArgs e)
        {
            PlaceOrder();
        }

        private void PlaceOrder()
        {
            if (!_cartItems.Any())
            {
                MessageBox.Show("Your cart is empty. Please add some items before placing an order.",
                    "Empty Cart", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check if payment method is selected
            if (_selectedPaymentMethodId == null)
            {
                MessageBox.Show("Please select a payment method before placing your order.",
                    "Payment Method Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_currentTable == null)
            {
                MessageBox.Show("Table information not found. Please contact staff.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var totalAmount = _cartItems.Sum(item => item.Product.Price * item.Quantity);

                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    StaffUserId = _loggedInUser?.UserId ?? 1, // Use staff user if available, otherwise use default
                    TableId = _currentTable.TableId,
                    TotalPrice = totalAmount,
                    OrderStatusId = 1, // Pending status
                    OrderDetails = new List<OrderDetail>()
                };

                foreach (var cartItem in _cartItems)
                {
                    order.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = cartItem.Product.ProductId,
                        Quantity = cartItem.Quantity
                    });
                }

                if (_orderService.AddOrder(order))
                {
                    // Create payment record
                    var payment = new Payment
                    {
                        OrderId = order.OrderId,
                        PaymentAmount = totalAmount,
                        PaymentDate = DateTime.Now,
                        MethodId = _selectedPaymentMethodId.Value
                    };

                    _paymentService.AddPayments(payment);

                    string paymentMethodName = _selectedPaymentMethodId == 1 ? "Cash" : "Bank Transfer";
                    string message = _loggedInUser != null
                        ? $"Order has been placed successfully with {paymentMethodName} payment! Staff will prepare the items shortly."
                        : $"Your order has been submitted successfully with {paymentMethodName} payment! Our staff will prepare your items and bring them to your table shortly. You can continue ordering more items if needed.";

                    MessageBox.Show(message, "Order Placed", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Clear cart and reset UI
                    _cartItems.Clear();
                    CartOverlay.Visibility = Visibility.Collapsed;
                    UpdateCartDisplay();
                    ResetPaymentSelection();

                    // Stay on the same page for continued ordering
                    // Remove the dialog asking if user wants to place another order
                }
                else
                {
                    MessageBox.Show("Failed to place order. Please try again or contact staff.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error placing order: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CashPayment_Click(object sender, MouseButtonEventArgs e)
        {
            // Check if cart is empty
            if (!_cartItems.Any())
            {
                MessageBox.Show("Your cart is empty. Please add some items before selecting a payment method.",
                    "Empty Cart", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectPaymentMethod(1, CashRadioButton, CashPaymentBorder); // Assuming Cash is method ID 1
        }

        private void BankTransfer_Click(object sender, MouseButtonEventArgs e)
        {
            // Check if cart is empty
            if (!_cartItems.Any())
            {
                MessageBox.Show("Your cart is empty. Please add some items before selecting a payment method.",
                    "Empty Cart", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectPaymentMethod(2, BankTransferRadioButton, BankTransferBorder); // Assuming Bank Transfer is method ID 2

            // Show QR Code overlay for bank transfer
            ShowQRCodeForBankTransfer();
        }

        private void ShowQRCodeForBankTransfer()
        {
            // Calculate and display transfer amount
            var totalAmount = _cartItems.Sum(item => item.Product.Price * item.Quantity);
            TransferAmountTextBlock.Text = $"{totalAmount:N0} VND";

            System.Diagnostics.Debug.WriteLine($"Bank transfer initiated for amount: {totalAmount:N0} VND");

            // Show bank transfer overlay
            QRCodeOverlay.Visibility = Visibility.Visible;
        }

        private void CloseQRButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide QR overlay and reset payment selection
            QRCodeOverlay.Visibility = Visibility.Collapsed;
            ResetPaymentSelection();
        }

        private void PaymentCompleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide transfer overlay - keep payment method selected
            QRCodeOverlay.Visibility = Visibility.Collapsed;

            // Show confirmation message
            MessageBox.Show("✅ Bank transfer method confirmed! You can now proceed to place your order.",
                "Payment Method Selected", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SelectPaymentMethod(int methodId, RadioButton radioButton, Border border)
        {
            _selectedPaymentMethodId = methodId;

            // Update radio button
            radioButton.IsChecked = true;

            // Update visual feedback
            ResetPaymentBorderStyles();
            border.Background = new SolidColorBrush(Color.FromRgb(255, 235, 179)); // Light orange background
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 107, 53)); // Orange border
            border.BorderThickness = new Thickness(2);
        }

        private void ResetPaymentBorderStyles()
        {
            // Reset all payment option borders
            CashPaymentBorder.Background = Brushes.White;
            CashPaymentBorder.BorderBrush = Brushes.Transparent;
            CashPaymentBorder.BorderThickness = new Thickness(0);

            BankTransferBorder.Background = Brushes.White;
            BankTransferBorder.BorderBrush = Brushes.Transparent;
            BankTransferBorder.BorderThickness = new Thickness(0);
        }

        private void ResetPaymentSelection()
        {
            _selectedPaymentMethodId = null;
            CashRadioButton.IsChecked = false;
            BankTransferRadioButton.IsChecked = false;
            ResetPaymentBorderStyles();
        }

        private void PerformLogout()
        {
            const string ADMIN_PASSWORD = "12345"; // Fixed admin password

            if (LogoutPasswordBox.Password == ADMIN_PASSWORD)
            {
                // Password correct - proceed with logout
                var result = MessageBox.Show("Are you sure you want to logout and return to the login screen?",
                    "Logout Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Clear password and hide dialog
                    LogoutPasswordBox.Clear();
                    LogoutPasswordOverlay.Visibility = Visibility.Collapsed;

                    // Return to login window
                    var loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
                else
                {
                    // User cancelled, just hide dialog
                    LogoutPasswordBox.Clear();
                    LogoutPasswordOverlay.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                // Wrong password
                MessageBox.Show("Incorrect password. Please try again.",
                    "Access Denied", MessageBoxButton.OK, MessageBoxImage.Error);
                LogoutPasswordBox.Clear();
                LogoutPasswordBox.Focus();
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Show password dialog
            LogoutPasswordOverlay.Visibility = Visibility.Visible;
            LogoutPasswordBox.Focus();
        }

        private void CancelLogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide password dialog and clear password
            LogoutPasswordOverlay.Visibility = Visibility.Collapsed;
            LogoutPasswordBox.Clear();
        }

        private void ConfirmLogoutButton_Click(object sender, RoutedEventArgs e)
        {
            PerformLogout();
        }

        private void LogoutPasswordBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                PerformLogout();
            }
        }
    }

    // CartItem class for managing cart items
    public class CartItem : System.ComponentModel.INotifyPropertyChanged
    {
        private int _quantity;

        public Product Product { get; set; }

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
        //Fix v2
    }
}
