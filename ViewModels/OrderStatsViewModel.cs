using System.ComponentModel;
using System.Windows.Input;
using TransConnect.DbContexts;
using TransConnect.Models;

namespace TransConnect.ViewModels
{
    public class OrderStatsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public BindingList<Order> Orders { get; set; } = new BindingList<Order>();

        public DateTime SelectedFrom { get; set; } = DateTime.Now.Date;

        // SelectedTo (inclusive)
        public DateTime SelectedTo { get; set; } = DateTime.Now.Date;

        public string MinTotal => Orders.Count > 0 ? Orders.Min(o => o.Price).ToString("0.00") : "";

        public string MaxTotal => Orders.Count > 0 ? Orders.Max(o => o.Price).ToString("0.00") : "";

        // Average total price of all orders (2 decimal places)
        public string AvgTotal => Orders.Count > 0 ? Orders.Average(o => o.Price).ToString("0.00") : "";

        public OrderStatsViewModel()
        {
        }

        public ICommand SearchCommand => new RelayCommand(_ =>
        {
            Orders.Clear();
            using (var db = new TransConnectDbContext())
            {
                var orders = db.Orders
                    .Where(o => o.CreationDate >= SelectedFrom && o.CreationDate <= SelectedTo.AddDays(1))
                    .ToList();
                Orders = new BindingList<Order>(orders);
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Orders)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MinTotal)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxTotal)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AvgTotal)));
        });
    }
}
