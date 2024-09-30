using TransConnect.Models;

namespace TransConnect.ViewModels
{
    public class StatusDescription
    {
        public OrderStatus Status { get; set; }
        public string Description { get; set; }

        public StatusDescription(OrderStatus status, string description)
        {
            Status = status;
            Description = description;
        }
        public override string ToString()
        {
            return Description;
        }

        public static readonly List<StatusDescription> StatusDescriptions = new List<StatusDescription>
        {
            new StatusDescription(OrderStatus.WAITING_FOR_CONFIRMATION, "WAITING_FOR_CONFIRMATION"),
            new StatusDescription(OrderStatus.CONFIRMED, "CONFIRMED"),
            new StatusDescription(OrderStatus.IN_PREPARATION, "IN_PREPARATION"),
            new StatusDescription(OrderStatus.DELIVERING, "DELIVERING"),
            new StatusDescription(OrderStatus.COMPLETED, "COMPLETED"),
            new StatusDescription(OrderStatus.CANCELLED, "CANCELLED")
        };
    }
}
