using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransConnect.DbContexts;
using TransConnect.Models;

namespace TransConnect.ViewModels
{
    public enum SelectionStrategyType
    {
        BY_TOTAL_SUCCESS_ORDER,
        BY_TOTAL_AMOUNT_SPENT,
    }

    public class SelectionStrategy
    {
        public SelectionStrategyType Type { get; set; }
        public string Description { get; set; }

        public static List<SelectionStrategy> Strategies = new List<SelectionStrategy>
        {
            new SelectionStrategy
            {
                Type = SelectionStrategyType.BY_TOTAL_SUCCESS_ORDER,
                Description = "By Total Successful Orders"
            },
            new SelectionStrategy
            {
                Type = SelectionStrategyType.BY_TOTAL_AMOUNT_SPENT,
                Description = "By Total Amount Spent"
            }
        };

        public override string ToString()
        {
            return Description;
        }
    }

    public class ClientRankingViewModel : INotifyPropertyChanged
    {

        public class RankingRow
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public BindingList<RankingRow> Rows { get; set; }

        public List<SelectionStrategy> Strategies { get; set; } = SelectionStrategy.Strategies;

        private SelectionStrategy _selectedStrategy;

        public SelectionStrategy SelectedStrategy
        {
            get => _selectedStrategy;
            set
            {
                _selectedStrategy = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedStrategy)));

                if (value.Type == SelectionStrategyType.BY_TOTAL_SUCCESS_ORDER)
                {
                    using (var db = new TransConnectDbContext())
                    {
                        // get a list of clients ordered by total successful orders
                        // and create a list of RankingRow objects
                        var clients = db.Clients.OrderByDescending(c => c.Orders.Count(o => o.Status == OrderStatus.COMPLETED)).ToList();
                        Rows = new BindingList<RankingRow>(clients.Select(c => new RankingRow
                        {
                            Id = c.Id.ToString(),
                            Name = c.FullName,
                            Value = c.Orders.Count(o => o.Status == OrderStatus.COMPLETED).ToString()
                        }).ToList());
                    }
                }
                else if (value.Type == SelectionStrategyType.BY_TOTAL_AMOUNT_SPENT)
                {
                    using (var db = new TransConnectDbContext())
                    {
                        // get a list of clients ordered by total amount spent (order is completed)
                        // and create a list of RankingRow objects
                        var clients = db.Clients.OrderByDescending(c => c.Orders.Where(o => o.Status == OrderStatus.COMPLETED).Sum(o => o.Price)).ToList();
                        Rows = new BindingList<RankingRow>(clients.Select(c => new RankingRow
                        {
                            Id = c.Id.ToString(),
                            Name = c.FullName,
                            // 2 decimal places
                            Value = c.Orders.Where(o => o.Status == OrderStatus.COMPLETED).Sum(o => o.Price).ToString("0.00")
                        }).ToList());
                    }
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Rows)));
            }
        }

        public ClientRankingViewModel()
        {
            SelectedStrategy = Strategies.First();
        }
    }
}
