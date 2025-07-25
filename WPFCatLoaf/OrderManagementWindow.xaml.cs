using BusinessLayer.IService;
using BusinessLayer.Service;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WPFCatLoaf
{
    public partial class OrderManagementWindow : Window
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IRestaurantTableService _tableService;
        private readonly IOrderDetailService _orderDetailService;

        public OrderManagementWindow()
        {
            InitializeComponent();
            var context = new LoafNcattingDbContext();
            _orderService = new OrderService(new OrderRepository(context));
            _userService = new UserService(new UserRepository(context));
            _tableService = new RestaurantTableService(new RestaurantTableRepository(context));
            _orderDetailService = new OrderDetailService(new OrderDetailRepository(context));

            LoadOrders();
        }

        private void LoadOrders()
        {
            var orders = _orderService.GetAllOrders();
            var users = _userService.GetAllUsers();
            var tables = _tableService.GetAllRestaurantTables();

            // Sử dụng kiểu ẩn danh để kết hợp dữ liệu
            var ordersToDisplay = orders.Select(o => new
            {
                o.OrderId,
                o.OrderDate,
                o.TotalPrice,
                StaffName = users.FirstOrDefault(u => u.UserId == o.StaffUserId)?.Name,
                TableName = tables.FirstOrDefault(t => t.TableId == o.TableId)?.TableName
            }).ToList();

            OrdersDataGrid.ItemsSource = ordersToDisplay;
        }

        private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem == null) return;

            // Lấy OrderId từ đối tượng ẩn danh đã chọn
            var selectedOrder = (dynamic)OrdersDataGrid.SelectedItem;
            int orderId = selectedOrder.OrderId;

            // Lấy chi tiết đơn hàng
            var orderDetails = _orderDetailService.GetByOrderId(orderId);

            // Tạo một danh sách kiểu ẩn danh khác cho chi tiết đơn hàng để tính SubTotal
            var detailsToDisplay = orderDetails.Select(od => new
            {
                ProductName = od.Product.Name,
                od.Quantity,
                SubTotal = od.Quantity * od.Product.Price
            }).ToList();

            OrderDetailsDataGrid.ItemsSource = detailsToDisplay;
        }
    }
}