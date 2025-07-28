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
            try
            {
                PaymentMethodSeries = new SeriesCollection();

                // Debug: Log all payments and orders first
                var allOrders = _orderService.GetAllOrders();
                var allPayments = _paymentService.GetAllPayments();
                
                System.Diagnostics.Debug.WriteLine($"=== PAYMENT DEBUG INFO ===");
                System.Diagnostics.Debug.WriteLine($"Total Orders: {allOrders.Count()}");
                System.Diagnostics.Debug.WriteLine($"Total Payments: {allPayments.Count()}");
                
                // Show order statuses
                foreach (var statusGroup in allOrders.GroupBy(o => o.OrderStatusId))
                {
                    System.Diagnostics.Debug.WriteLine($"Status {statusGroup.Key}: {statusGroup.Count()} orders");
                }
                
                // Show payment methods
                foreach (var methodGroup in allPayments.GroupBy(p => p.MethodId))
                {
                    System.Diagnostics.Debug.WriteLine($"Method {methodGroup.Key}: {methodGroup.Count()} payments");
                }

                var completedStatusId = 4; // StatusId cho đơn hàng đã hoàn thành
                
                // Lấy thống kê payment methods từ TẤT CẢ các đơn hàng thay vì chỉ completed
                var allOrderIds = allOrders
                    .Select(o => o.OrderId)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Using All Orders: {allOrderIds.Count}");

                var payments = allPayments
                    .Where(p => allOrderIds.Contains(p.OrderId))
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Matching Payments: {payments.Count}");

                if (!payments.Any())
                {
                    // Nếu không có payment data, tạo chart trống
                    PaymentMethodSeries.Add(new PieSeries
                    {
                        Title = "No Payment Data",
                        Values = new ChartValues<int> { 1 },
                        DataLabels = true,
                        Fill = System.Windows.Media.Brushes.LightGray
                    });
                    return;
                }

                // Group payments by MethodId and map to method names
                var paymentGroups = payments
                    .GroupBy(p => p.MethodId)
                    .Select(g => new { 
                        MethodId = g.Key, 
                        Count = g.Count(),
                        MethodName = GetPaymentMethodName(g.Key)
                    })
                    .OrderByDescending(g => g.Count)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Payment Groups: {paymentGroups.Count}");
                foreach (var group in paymentGroups)
                {
                    System.Diagnostics.Debug.WriteLine($"  {group.MethodName} (ID: {group.MethodId}): {group.Count}");
                }

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
                        LabelPoint = point => $"{group.MethodName}: {point.Y} ({point.Participation:P0})",
                        Fill = colors[colorIndex % colors.Length]
                    });
                    colorIndex++;
                }
            }
            catch (Exception ex)
            {
                // Handle error case
                PaymentMethodSeries = new SeriesCollection();
                PaymentMethodSeries.Add(new PieSeries
                {
                    Title = "Error Loading Data",
                    Values = new ChartValues<int> { 1 },
                    DataLabels = true,
                    Fill = System.Windows.Media.Brushes.Red
                });
                
                MessageBox.Show($"Error loading payment methods chart: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                System.Diagnostics.Debug.WriteLine($"Payment Chart Error: {ex.Message}");
            }
        }

        private string GetPaymentMethodName(int methodId)
        {
            return methodId switch
            {
                1 => "Cash",
                2 => "Bank Transfer", 
                3 => "Credit Card",
                4 => "E-Wallet",
                5 => "Debit Card",
                _ => $"Method {methodId}"
            };
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