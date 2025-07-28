using BusinessLayer.IService;
using BusinessLayer.Service;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System;

namespace WPFCatLoaf
{
    public partial class OrderManagementWindow : Window
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IRestaurantTableService _tableService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IPaymentService _paymentService;
        private readonly LoafNcattingDbContext _context;
        private readonly User _currentUser;
        private DispatcherTimer _timer;

        public OrderManagementWindow(User currentUser)
        {
            InitializeComponent();
            _context = new LoafNcattingDbContext();
            _orderService = new OrderService(new OrderRepository(_context));
            _userService = new UserService(new UserRepository(_context));
            _tableService = new RestaurantTableService(new RestaurantTableRepository(_context));
            _orderDetailService = new OrderDetailService(new OrderDetailRepository(_context));
            _paymentService = new PaymentService(new PaymentRepository(_context));
            _currentUser = currentUser;

            SetupTimer();
            LoadOrders();
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

        private void LoadOrders()
        {
            var allOrders = _orderService.GetAllOrders();
            var users = _userService.GetAllUsers();
            var tables = _tableService.GetAllRestaurantTables();
            var allStatuses = _context.OrderStatuses.ToList();
            var allPayments = _paymentService.GetAllPayments();

            List<Order> filteredOrders;

            if (_currentUser.RoleId == 3) // Staff
            {
                filteredOrders = allOrders.Where(o => o.OrderStatusId == 3).ToList();
            }
            else // Admin
            {
                filteredOrders = allOrders.Where(o => new List<int> { 3, 4, 5 }.Contains(o.OrderStatusId)).ToList();
            }

            var ordersToDisplay = filteredOrders.Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                StaffName = users.FirstOrDefault(u => u.UserId == o.StaffUserId)?.Name,
                TableName = tables.FirstOrDefault(t => t.TableId == o.TableId)?.TableName,
                PaymentMethod = GetPaymentMethodForOrder(o.OrderId, allPayments),
                OrderStatuses = allStatuses,
                SelectedOrderStatus = allStatuses.FirstOrDefault(s => s.OrderStatusId == o.OrderStatusId)
            }).ToList();

            OrdersDataGrid.ItemsSource = ordersToDisplay;
        }

        private string GetPaymentMethodForOrder(int orderId, List<Payment> allPayments)
        {
            var payment = allPayments.FirstOrDefault(p => p.OrderId == orderId);
            if (payment != null)
            {
                // Assuming MethodId 1 = Cash, 2 = Bank Transfer
                switch (payment.MethodId)
                {
                    case 1:
                        return "💵 Cash";
                    case 2:
                        return "🏦 Bank Transfer";
                    default:
                        return "❓ Unknown";
                }
            }
            return "❌ No Payment";
        }

        private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is OrderViewModel selectedOrder)
            {
                var orderDetails = _orderDetailService.GetByOrderId(selectedOrder.OrderId);
                var detailsToDisplay = orderDetails.Select(od => new
                {
                    ProductName = od.Product.Name,
                    od.Quantity,
                    SubTotal = od.Quantity * od.Product.Price
                }).ToList();
                OrderDetailsDataGrid.ItemsSource = detailsToDisplay;
            }
        }

        private void OrderStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).DataContext is OrderViewModel selectedOrderViewModel &&
                selectedOrderViewModel.SelectedOrderStatus != null)
            {
                var orderToUpdate = _orderService.GetOrderById(selectedOrderViewModel.OrderId);
                if (orderToUpdate != null)
                {
                    orderToUpdate.OrderStatusId = selectedOrderViewModel.SelectedOrderStatus.OrderStatusId;
                    _orderService.UpdateOrder(orderToUpdate);
                }
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            _timer?.Stop();
            var mainMenuWindow = new MainMenuWindow(_currentUser);
            mainMenuWindow.Show();
            this.Close();
        }

        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            _timer?.Stop();
            var orderWindow = new OrderWindow(_currentUser);
            orderWindow.Show();
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer?.Stop();
            base.OnClosed(e);
        }
    }

    public class OrderViewModel : INotifyPropertyChanged
    {
        public int OrderId { get; set; }
        public System.DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string StaffName { get; set; }
        public string TableName { get; set; }
        public string PaymentMethod { get; set; }
        public List<OrderStatus> OrderStatuses { get; set; }

        private OrderStatus _selectedOrderStatus;
        public OrderStatus SelectedOrderStatus
        {
            get => _selectedOrderStatus;
            set
            {
                if (_selectedOrderStatus != value)
                {
                    _selectedOrderStatus = value;
                    OnPropertyChanged(nameof(SelectedOrderStatus));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
