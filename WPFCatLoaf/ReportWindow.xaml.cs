using BusinessLayer.IService;
using BusinessLayer.Service;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace WPFCatLoaf
{
    public partial class ReportWindow : Window
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IProductService _productService;
        private readonly ICatService _catService;
        private readonly LoafNcattingDbContext _context;
        private readonly User _loggedInUser;
        private DispatcherTimer _timer;

        public SeriesCollection RevenueSeries { get; set; }
        public string[] RevenueLabels { get; set; }
        public Func<double, string> CurrencyFormatter { get; set; }

        public SeriesCollection PaymentMethodSeries { get; set; }

        public ReportWindow(User user)
        {
            InitializeComponent();
            _loggedInUser = user;

            _context = new LoafNcattingDbContext();
            _paymentService = new PaymentService(new PaymentRepository(_context));
            _orderService = new OrderService(new OrderRepository(_context));
            _orderDetailService = new OrderDetailService(new OrderDetailRepository(_context));
            _productService = new ProductService(new ProductRepository(_context));
            _catService = new CatService(new CatRepository(_context));

            CurrencyFormatter = value => value.ToString("C");

            SetupTimer();
            LoadAllReports();
            DataContext = this;
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

        private void LoadAllReports()
        {
            LoadKpis();
            LoadRevenueChart();
            LoadTopSellingProducts();
            LoadPaymentMethodsChart();
        }

        private void LoadKpis()
        {
            // Cập nhật OrderStatus IDs theo yêu cầu:
            // 3 = Đang xử lý (Processing/Preparing)
            // 4 = Đã hoàn thành (Completed) 
            // 5 = Đã hủy (Cancelled)
            
            var allOrders = _orderService.GetAllOrders();
            var allOrderDetails = _orderDetailService.GetAllOrderDetails();
            var cats = _catService.GetAllCats();

            // Tính toán các loại orders theo status
            var completedOrders = allOrders.Where(o => o.OrderStatusId == 4).ToList();
            var cancelledOrders = allOrders.Where(o => o.OrderStatusId == 5).ToList();
            var processingOrders = allOrders.Where(o => o.OrderStatusId == 3).ToList();

            // Tính tổng doanh thu từ các đơn hàng đã hoàn thành
            var totalRevenue = completedOrders.Sum(o => o.TotalPrice);
            
            // KPI Cards - Row 1
            TotalRevenueTextBlock.Text = totalRevenue.ToString("C");
            TotalOrdersTextBlock.Text = allOrders.Count().ToString(); // Tất cả orders            
            
            // Tính tổng sản phẩm đã bán từ các đơn hàng đã hoàn thành
            var completedOrderIds = completedOrders.Select(o => o.OrderId).ToList();
            var completedOrderDetails = allOrderDetails.Where(od => completedOrderIds.Contains(od.OrderId)).ToList();
            TotalProductsSoldTextBlock.Text = completedOrderDetails.Sum(od => od.Quantity).ToString();
            
            // Đếm số mèo đang hoạt động (StatusId = 1 thường là "Active")
            TotalCatsTextBlock.Text = cats.Count(c => c.StatusId == 1).ToString();

            // KPI Cards - Row 2 (New)
            CompletedOrdersTextBlock.Text = completedOrders.Count().ToString();
            CancelledOrdersTextBlock.Text = cancelledOrders.Count().ToString();
            ProcessingOrdersTextBlock.Text = processingOrders.Count().ToString();
            
            // Tính tỷ lệ thành công (Completed / (Completed + Cancelled))
            var totalFinishedOrders = completedOrders.Count() + cancelledOrders.Count();
            var successRate = totalFinishedOrders > 0 ? (double)completedOrders.Count() / totalFinishedOrders * 100 : 0;
            SuccessRateTextBlock.Text = $"{successRate:F1}%";
        }

        private void LoadRevenueChart()
        {
            var completedStatusId = 4; // StatusId cho đơn hàng đã hoàn thành
            
            // Lấy doanh thu từ các đơn hàng đã hoàn thành trong 30 ngày qua
            var completedOrders = _orderService.GetAllOrders()
                .Where(o => o.OrderStatusId == completedStatusId && o.OrderDate >= DateTime.Now.AddDays(-30))
                .ToList();

            var dailyRevenue = completedOrders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(o => o.TotalPrice) })
                .OrderBy(d => d.Date)
                .ToList();

            RevenueSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Revenue",
                    Values = new ChartValues<decimal>(dailyRevenue.Select(d => d.Total)),
                    Stroke = System.Windows.Media.Brushes.Orange,
                    Fill = System.Windows.Media.Brushes.Transparent,
                    StrokeThickness = 3,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 8
                }
            };
            RevenueLabels = dailyRevenue.Select(d => d.Date.ToString("MMM dd")).ToArray();
        }

        private void LoadTopSellingProducts()
        {
            var completedStatusId = 4; // StatusId cho đơn hàng đã hoàn thành
            
            // Lấy top 5 sản phẩm bán chạy nhất từ các đơn hàng đã hoàn thành
            var completedOrders = _orderService.GetAllOrders()
                .Where(o => o.OrderStatusId == completedStatusId) // Chỉ lấy đơn hàng đã hoàn thành
                .Select(o => o.OrderId)
                .ToList();

            var topProducts = _orderDetailService.GetAllOrderDetails()
                .Where(od => completedOrders.Contains(od.OrderId)) // Chỉ lấy từ đơn hàng đã hoàn thành
                .GroupBy(od => od.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    QuantitySold = g.Sum(od => od.Quantity),
                    Revenue = g.Sum(od => od.Quantity * od.Product.Price)
                })
                .OrderByDescending(p => p.QuantitySold)
                .Take(5) // Chỉ lấy top 5
                .Join(_productService.GetAllProducts(),
                      g => g.ProductId,
                      p => p.ProductId,
                      (g, p) => new { ProductName = p.Name, g.QuantitySold, g.Revenue })
                .ToList();

            TopProductsDataGrid.ItemsSource = topProducts;
        }

        private void LoadPaymentMethodsChart()
        {
            PaymentMethodSeries = new SeriesCollection();

            var completedStatusId = 4; // StatusId cho đơn hàng đã hoàn thành
            
            // Lấy thống kê payment methods từ các đơn hàng đã hoàn thành
            var completedOrderIds = _orderService.GetAllOrders()
                .Where(o => o.OrderStatusId == completedStatusId)
                .Select(o => o.OrderId)
                .ToList();

            var payments = _paymentService.GetAllPayments()
                .Where(p => completedOrderIds.Contains(p.OrderId))
                .ToList();

            var paymentGroups = payments
                .GroupBy(p => p.Method.MethodName)
                .Select(g => new { MethodName = g.Key, Count = g.Count() })
                .ToList();

            var colors = new[]
            {
                System.Windows.Media.Brushes.Orange,
                System.Windows.Media.Brushes.DarkOrange,
                System.Windows.Media.Brushes.OrangeRed,
                System.Windows.Media.Brushes.Coral,
                System.Windows.Media.Brushes.SandyBrown
            };

            int colorIndex = 0;
            foreach (var group in paymentGroups)
            {
                PaymentMethodSeries.Add(new PieSeries
                {
                    Title = group.MethodName,
                    Values = new ChartValues<int> { group.Count },
                    DataLabels = true,
                    Fill = colors[colorIndex % colors.Length]
                });
                colorIndex++;
            }
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