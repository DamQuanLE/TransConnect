namespace TransConnect.Models
{
    public class VanOrder(Guid clientId, Guid startId, Guid endId, decimal price, string purpose) : Order(clientId, startId, endId, VehicleType.VAN, price)
    {
        public string Purpose { get; set; } = purpose;
    }
}
