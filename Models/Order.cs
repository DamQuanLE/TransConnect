using System.Collections.ObjectModel;

namespace TransConnect.Models
{
    public class Order(Guid clientId, Guid startId, Guid endId, VehicleType vehicleType, decimal price)
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ClientId { get; set; } = clientId;
        public virtual Client? Client { get; set; }
        public Guid StartId { get; set; } = startId;
        public virtual Point? Start { get; set; }
        public Guid EndId { get; set; } = endId;
        public virtual Point? End { get; set; }
        public Guid? DriverId { get; set; }
        public virtual Employee? Driver { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime? ConfirmationDate { get; set; }
        public DateTime? PreparationDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime? CancellationDate { get; set; }

        public Guid? VehicleId { get; set; }
        public virtual Vehicle? Vehicle { get; set; }
        public VehicleType VehicleType { get; set; } = vehicleType;
        // To be calculated based on the distance between the start and end points
        public decimal Price { get; set; } = price;
        public OrderStatus Status { get; set; } = OrderStatus.WAITING_FOR_CONFIRMATION;

        public virtual ICollection<OrderPath> OrderPaths { get; private set; } = new ObservableCollection<OrderPath>();

        public void Confirm() { Status = OrderStatus.CONFIRMED; ConfirmationDate = DateTime.Now; }
        public void Prepare() { Status = OrderStatus.IN_PREPARATION; PreparationDate = DateTime.Now; }
        public void Deliver() { Status = OrderStatus.DELIVERING; DeliveryDate = DateTime.Now; }
        public void Complete() { Status = OrderStatus.COMPLETED; CompletionDate = DateTime.Now; }
        public void Cancel() { Status = OrderStatus.CANCELLED; CancellationDate = DateTime.Now; }

        public override string ToString()
        {
            return $"{Id.ToString().Split('-')[0]} [{VehicleType}] ({CreationDate})";
        }
    }
}