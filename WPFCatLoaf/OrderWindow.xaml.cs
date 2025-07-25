using BusinessLayer.IService;
using BusinessLayer.Service;
using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System;

namespace WPFCatLoaf
{
    public partial class OrderWindow : Window
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IRestaurantTableService _tableService;

        private readonly User _loggedInStaff;
        private ObservableCollection<OrderDetailViewModel> _currentOrderItems;
        private DispatcherTimer _timer;

        public OrderWindow(User staffUser)
        {
            InitializeComponent();
            _loggedInStaff = staffUser;

            // Khởi tạo services
            var context = new LoafNcattingDbContext();
            _productService = new ProductService(new ProductRepository(context));
            _orderService = new OrderService(new OrderRepository(context));
            _orderDetailService = new OrderDetailService(new OrderDetailRepository(context));
            _tableService = new RestaurantTableService(new RestaurantTableRepository(context));

            // Khởi tạo giỏ hàng
            _currentOrderItems = new ObservableCollection<OrderDetailViewModel>();
            OrderDetailsDataGrid.ItemsSource = _currentOrderItems;

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
            // Nạp danh sách sản phẩm và bàn
            ProductsDataGrid.ItemsSource = _productService.GetAllProducts();
            TableComboBox.ItemsSource = _tableService.GetAllRestaurantTables();
        }

        private void AddToOrderButton_Click(object sender, RoutedEventArgs e)
        {
            // Lấy sản phẩm được chọn từ DataGrid
            if (((FrameworkElement)sender).DataContext is Product selectedProduct)
            {
                // Giả sử mỗi lần nhấn nút là thêm 1 sản phẩm
                var quantity = 1;

                // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
                var existingItem = _currentOrderItems.FirstOrDefault(item => item.ProductId == selectedProduct.ProductId);

                if (existingItem != null)
                {
                    // Nếu có, tăng số lượng
                    existingItem.Quantity += quantity;
                }
                else
                {
                    // Nếu chưa, thêm mới vào giỏ hàng
                    _currentOrderItems.Add(new OrderDetailViewModel
                    {
                        ProductId = selectedProduct.ProductId,
                        Quantity = quantity,
                        Product = selectedProduct
                    });
                }
                UpdateTotalPrice();
            }
        }

        private void UpdateTotalPrice()
        {
            decimal totalPrice = _currentOrderItems.Sum(item => item.SubTotal);
            TotalPriceTextBlock.Text = $"Total: {totalPrice:C}";
        }

        private void SubmitOrderButton_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra điều kiện
            if (!_currentOrderItems.Any())
            {
                MessageBox.Show("Please add items to the order.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (TableComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a table.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Tạo đối tượng Order
            var newOrder = new Order
            {
                OrderDate = System.DateTime.Now,
                StaffUserId = _loggedInStaff.UserId,
                TableId = (int)TableComboBox.SelectedValue,
                TotalPrice = _currentOrderItems.Sum(item => item.SubTotal),
                OrderDetails = new List<OrderDetail>()
            };

            // Thêm OrderDetails vào Order
            foreach (var item in _currentOrderItems)
            {
                newOrder.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            // Lưu Order (EF Core sẽ tự động lưu OrderDetails liên quan)
            if (_orderService.AddOrder(newOrder))
            {
                MessageBox.Show("Order created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                // Xóa giỏ hàng để chuẩn bị cho order tiếp theo
                _currentOrderItems.Clear();
                UpdateTotalPrice();
            }
            else
            {
                MessageBox.Show("Failed to create order.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManageOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            _timer?.Stop();
            var allOrdersWindow = new OrderManagementWindow(_loggedInStaff);
            allOrdersWindow.Show();
            this.Close();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            _timer?.Stop();
            var mainMenuWindow = new MainMenuWindow(_loggedInStaff);
            mainMenuWindow.Show();
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer?.Stop();
            base.OnClosed(e);
        }
    }

    // Một lớp ViewModel đơn giản để hiển thị trong giỏ hàng
    public class OrderDetailViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public int ProductId { get; set; }
        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(SubTotal));
            }
        }
        public Product Product { get; set; }
        public decimal SubTotal => Product.Price * Quantity;

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}