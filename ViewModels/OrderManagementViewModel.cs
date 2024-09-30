using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TransConnect.DbContexts;
using TransConnect.Models;

namespace TransConnect.ViewModels
{
    public class OrderManagementViewModel : INotifyPropertyChanged
    {

        public List<StatusDescription> Statuses { get; set; } = StatusDescription.StatusDescriptions;

        public event PropertyChangedEventHandler? PropertyChanged;

        private StatusDescription _selectedStatus;

        public StatusDescription SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                _selectedStatus = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedStatus"));

                using (var context = new TransConnectDbContext())
                {
                    var orders = context.Orders
                        .Where(o => o.Status == value.Status)
                        .Include(o => o.Client)
                        .Include(o => o.Start)
                        .Include(o => o.End)
                        .OrderByDescending(o => o.CreationDate)
                        .ToList();
                    Orders = new BindingList<Order>(orders);
                }
            }
        }

        private BindingList<Order> _orders;

        public BindingList<Order> Orders
        {
            get { return _orders; }
            set
            {
                _orders = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Orders"));
            }
        }

        private Order _selectedOrder;

        public Order SelectedOrder
        {
            get { return _selectedOrder; }
            set
            {
                if (value == null)
                {
                    _selectedOrder = null;
                } else
                {
                    // Get the order with orderpaths (with point names), driver info
                    using (var context = new TransConnectDbContext())
                    {
                        var order = context.Orders
                            .Where(o => o.Id == value.Id)
                            .Include(o => o.Client)
                            .Include(o => o.Start)
                            .Include(o => o.End)
                            .Include(o => o.OrderPaths)
                            .ThenInclude(op => op.From)
                            .Include(o => o.OrderPaths)
                            .ThenInclude(op => op.To)
                            .Include(o => o.Driver)
                            .Include(o => o.Vehicle)
                            .FirstOrDefault();

                        _selectedOrder = order;
                        VehicleDescription = VehicleTypeDescription.VehicleTypeDescriptions.Find(v => v.Type == order.VehicleType)!.Description;

                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("VehicleDescription"));

                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CarOrderVisibility"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TruckOrderVisibility"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("VanOrderVisibility"));

                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ConfirmVisibility"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PrepareVisibility"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DeliverVisibility"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CompleteVisibility"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DoneVisibility"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CancelVisibility"));

                        if (order.Status == OrderStatus.WAITING_FOR_CONFIRMATION)
                        {
                            fetchDrivers();
                            fetchVehicles();

                            PrepareVehicle = null;
                            PrepareDriver = null;

                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PrepareVehicle"));
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PrepareDriver"));
                        }
                    }
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedOrder"));

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OrderInfoVisibility"));
            }
        }

        public string OrderInfoVisibility => SelectedOrder == null ? "Collapsed" : "Visible";

        public string CarOrderVisibility => SelectedOrder?.VehicleType == VehicleType.CAR ? "Visible" : "Collapsed";
        public string VanOrderVisibility => SelectedOrder?.VehicleType == VehicleType.VAN ? "Visible" : "Collapsed";
        public string TruckOrderVisibility => (SelectedOrder?.VehicleType == VehicleType.TANKER_TRUCK || SelectedOrder?.VehicleType == VehicleType.DUMP_TRUCK || SelectedOrder?.VehicleType == VehicleType.REFRIGERATED_TRUCK) ? "Visible" : "Collapsed";

        public string ConfirmVisibility => SelectedOrder?.Status == OrderStatus.WAITING_FOR_CONFIRMATION ? "Visible" : "Collapsed";
        public string PrepareVisibility => SelectedOrder?.Status == OrderStatus.CONFIRMED ? "Visible" : "Collapsed";
        public string DeliverVisibility => SelectedOrder?.Status == OrderStatus.IN_PREPARATION ? "Visible" : "Collapsed";
        public string CompleteVisibility => SelectedOrder?.Status == OrderStatus.DELIVERING ? "Visible" : "Collapsed";
        public string DoneVisibility => SelectedOrder?.Status == OrderStatus.COMPLETED ? "Visible" : "Collapsed";
        public string CancelVisibility => SelectedOrder?.Status != OrderStatus.CANCELLED && SelectedOrder?.Status != OrderStatus.COMPLETED ? "Visible" : "Collapsed";
        public string VehicleDescription { get; set; } = "";

        public BindingList<Vehicle> Vehicles { get; set; }

        public BindingList<Employee> Drivers { get; set; }

        public Vehicle PrepareVehicle { get; set; }

        public Employee PrepareDriver { get; set; }

        public OrderManagementViewModel()
        {
            SelectedStatus = Statuses.First();
        }

        private void fetchDrivers()
        {
            using (var context = new TransConnectDbContext())
            {
                // find here position is driver and does not have any order today
                var drivers = context.Employees
                    .Where(e => e.Position == "Driver")
                    .Where(e => !context.Orders.Any(o => o.DriverId == e.Id && o.Status != OrderStatus.CANCELLED && o.Status != OrderStatus.COMPLETED && o.CreationDate.Date <= DateTime.Now.Date))
                    .OrderBy(e => e.FirstName)
                    .ToList();
                Drivers = new BindingList<Employee>(drivers);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Drivers"));
            }
        }

        private void fetchVehicles()
        {
            using (var context = new TransConnectDbContext())
            {
                
                List<Vehicle> vehicles = new List<Vehicle>();

                switch (SelectedOrder.VehicleType)
                {
                    case VehicleType.CAR:
                        vehicles = context.Cars.ToList<Vehicle>();
                        break;
                    case VehicleType.VAN:
                        vehicles = context.Vans.ToList<Vehicle>();
                        break;
                    case VehicleType.TANKER_TRUCK:
                        vehicles = context.TankerTrucks.ToList<Vehicle>();
                        break;
                    case VehicleType.DUMP_TRUCK:
                        vehicles = context.DumpTrucks.ToList<Vehicle>();
                        break;
                    case VehicleType.REFRIGERATED_TRUCK:
                        vehicles = context.RefrigeratedTrucks.ToList<Vehicle>();
                        break;
                }
                Vehicles = new BindingList<Vehicle>(vehicles);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Vehicles"));
            }
        }

        public ICommand NextStateCommand => new RelayCommand( _ => {
            if (MessageBox.Show("Are you sure you want to change the status of this order?", "Change Order Status", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            switch (SelectedOrder.Status)
            {
                case OrderStatus.WAITING_FOR_CONFIRMATION:
                    if (PrepareDriver == null || PrepareVehicle == null)
                    {
                        MessageBox.Show("Please select a driver and a vehicle for the order.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    SelectedOrder.DriverId = PrepareDriver.Id;
                    SelectedOrder.VehicleId = PrepareVehicle.Id;

                    SelectedOrder.Confirm();
                    break;
                case OrderStatus.CONFIRMED:
                    SelectedOrder.Prepare();
                    break;
                case OrderStatus.IN_PREPARATION:
                    SelectedOrder.Deliver();
                    break;
                case OrderStatus.DELIVERING:
                    SelectedOrder.Complete();
                    break;
            }
            using (var context = new TransConnectDbContext())
            {
                context.Orders.Update(SelectedOrder);
                context.SaveChanges();
            }

            MessageBox.Show("Order status updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            SelectedStatus = SelectedStatus;
        });

        public ICommand CancelCommand => new RelayCommand(_ =>
        {
            if (MessageBox.Show("Are you sure you want to cancel this order?", "Cancel Order", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            SelectedOrder.Cancel();
            using (var context = new TransConnectDbContext())
            {
                context.Orders.Update(SelectedOrder);
                context.SaveChanges();
            }

            MessageBox.Show("Order cancelled successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            SelectedStatus = SelectedStatus;
        });


    }
}
