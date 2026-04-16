namespace WebshopShop.ViewModels.Admin
{
    public class StatisticsViewModel
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalUsers { get; set; }
        public int TotalItems { get; set; }
        public List<TopItemViewModel> TopItems { get; set; } = [];
        public List<RevenueByDayViewModel> RevenueByDay { get; set; } = [];
    }

    public class TopItemViewModel
    {
        public string Title { get; set; } = string.Empty;
        public int SaleCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class RevenueByDayViewModel
    {
        public string Day { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }
}