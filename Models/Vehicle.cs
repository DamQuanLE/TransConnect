using System.Collections.ObjectModel;

namespace TransConnect.Models
{
    public enum VehicleType
    {
        CAR,
        VAN,
        DUMP_TRUCK,
        TANKER_TRUCK,
        REFRIGERATED_TRUCK,
    }

    public class Vehicle
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public virtual ICollection<Order> Orders { get; private set; } = new ObservableCollection<Order>();
        public override string ToString()
        {
            return $"{Id.ToString().Split('-')[0]}";
        }
    }
}