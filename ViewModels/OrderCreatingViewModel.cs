using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TransConnect.DbContexts;
using TransConnect.Models;

namespace TransConnect.ViewModels
{
    public class OrderCreatingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public List<Client> Clients { get; set; }

        public Client SelectedClient { get; set; }

        public List<Models.Point> Points { get; set; }

        public Models.Point SelectedStartPoint { get; set; }

        public Models.Point SelectedEndPoint { get; set; }

        public Order PrepareOrder { get; set; }

        public List<VehicleTypeDescription> VehicleTypes { get; set; }

        private VehicleTypeDescription _selectedVehicleType;
        public VehicleTypeDescription SelectedVehicleType {
            get => _selectedVehicleType;
            set
            {
                _selectedVehicleType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedVehicleType"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PriceLists"));
            }
        }

        public decimal CalculatedPrice { get; set; }

        public decimal CalculatedDistance { get; set; }

        public List<PriceTypeDescription> PriceTypes { get; set; }

        private PriceTypeDescription _selectedPriceType;
        public PriceTypeDescription SelectedPriceType
        {
            get => _selectedPriceType;
            set
            {
                _selectedPriceType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedPriceType"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PriceLists"));
            }
        }

        public string NoRouteVisibility { get; set; } = "Collapsed";

        private BindingList<PriceList> _priceLists;
        public BindingList<PriceList> PriceLists {
            get
            {
                if (_selectedPriceType != null && _selectedVehicleType != null)
                {
                    using (var context = new TransConnectDbContext())
                    {
                        _priceLists = new BindingList<PriceList>(context.PriceList
                            .Where(p => p.PriceType == SelectedPriceType.Type)
                            .Where(p => p.VehicleType == SelectedVehicleType.Type)
                            .OrderBy(p => p.From)
                            .ToList());
                    }
                }
                return _priceLists;
            }
        }

        public int NumOfPassengers { get; set; }
        public string Purpose { get; set; }
        public double Volume { get; set; }
        public string Materials { get; set; }

        public OrderCreatingViewModel()
        {
            Clients = new List<Client>();
            Points = new List<Models.Point>();

            using (var context = new TransConnectDbContext())
            {
                Clients = context.Clients.OrderBy(c => c.FirstName).ToList();
                Points = context.Points.OrderBy(p => p.Name).ToList();
            }

            SelectedStartPoint = Points.First();
            SelectedEndPoint = Points.First();

            PriceTypes = PriceTypeDescription.PriceTypeDescriptions;

            SelectedPriceType = PriceTypes.First();

            VehicleTypes = VehicleTypeDescription.VehicleTypeDescriptions;

            SelectedVehicleType = VehicleTypes.First();


        }

        private List<Models.Point>? calculatePrice()
        {
            if (SelectedPriceType == null || SelectedVehicleType == null || SelectedStartPoint == null || SelectedEndPoint == null || SelectedStartPoint == SelectedEndPoint)
            {
                MessageBox.Show("Incorrect input data");
                CalculatedPrice = 0;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CalculatedPrice"));
                CalculatedDistance = 0;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CalculatedDistance"));
                return null;
            }

            using (var context = new TransConnectDbContext())
            {
                var priceLists = context.PriceList
                    .Where(p => p.PriceType == SelectedPriceType.Type)
                    .Where(p => p.VehicleType == SelectedVehicleType.Type)
                    .OrderBy(p => p.From)
                    .ToList();

                var res = Algos.Routing.Dijkstra(SelectedStartPoint, SelectedEndPoint, SelectedPriceType.Type);
                if (res == null)
                {
                    NoRouteVisibility = "Visible";
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoRouteVisibility"));
                    CalculatedPrice = 0;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CalculatedPrice"));
                    CalculatedDistance = 0;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CalculatedDistance"));
                    return null;
                }
                var distance = new decimal(res.Item1);
                var paths = res.Item2;

                var total = 0M;
                
                for (int i = 0; i < priceLists.Count; i++)
                {
                    if (distance >= priceLists[i].From)
                    {
                        if (i == priceLists.Count - 1)
                        {
                            total += (distance - priceLists[i].From) * priceLists[i].UnitPrice;
                        }
                        else
                        {
                            total += (priceLists[i + 1].From - priceLists[i].From) * priceLists[i].UnitPrice;
                        }
                    }
                }

                CalculatedPrice = total;
                CalculatedDistance = distance;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CalculatedPrice"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CalculatedDistance"));
                NoRouteVisibility = "Collapsed";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoRouteVisibility"));

                return paths;
            }
        }

        public ICommand CalculatePriceCommand => new RelayCommand(_ =>
        {
            calculatePrice();
        });

        public ICommand CreateOrderCommand => new RelayCommand(_ =>
        {
            if (SelectedClient == null || SelectedStartPoint == null || SelectedEndPoint == null || SelectedPriceType == null || SelectedVehicleType == null || SelectedStartPoint == SelectedEndPoint)
            {
                MessageBox.Show("Incorrect input data");
                return;
            }

            var paths = calculatePrice();

            if (CalculatedPrice == 0 || CalculatedDistance == 0 || paths == null)
            {
                MessageBox.Show("No route found!");
                return;
            }

            // Confirm order
            var result = MessageBox.Show("Do you want to create this order (Price: " + CalculatedPrice + ")?", "Confirm order", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            using (var context = new TransConnectDbContext())
            {
                Order order;
                switch (SelectedVehicleType.Type)
                {
                    case VehicleType.CAR:
                        order = new CarOrder(SelectedClient.Id, SelectedStartPoint.Id, SelectedEndPoint.Id, CalculatedPrice, NumOfPassengers);
                        break;
                    case VehicleType.VAN:
                        order = new VanOrder(SelectedClient.Id, SelectedStartPoint.Id, SelectedEndPoint.Id, CalculatedPrice, Purpose);
                        break;
                    default:
                        order = new HeavyTruckOrder(SelectedClient.Id, SelectedStartPoint.Id, SelectedEndPoint.Id, SelectedVehicleType.Type, CalculatedPrice, Materials, Volume);
                        break;
                }

                for (int i = 0; i < paths.Count - 1; i++)
                {
                    order.OrderPaths.Add(new OrderPath(order.Id, paths[i].Id, paths[i + 1].Id, i + 1));
                }

                context.Orders.Add(order);
                context.SaveChanges();

                MessageBox.Show("Order created successfully!");

                // Clear fields
                SelectedClient = null;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedClient"));
                SelectedStartPoint = Points.First();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedStartPoint"));
                SelectedEndPoint = Points.First();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedEndPoint"));
                SelectedPriceType = PriceTypes.First();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedPriceType"));
                SelectedVehicleType = VehicleTypes.First();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedVehicleType"));
                NumOfPassengers = 0;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NumOfPassengers"));
                Purpose = "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Purpose"));
                Volume = 0;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Volume"));
                Materials = "";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Materials"));
                CalculatedPrice = 0;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CalculatedPrice"));
                CalculatedDistance = 0;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CalculatedDistance"));
            }
        });


    }
}
