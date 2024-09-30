namespace TransConnect.Models
{
    public class HeavyTruckOrder(Guid clientId, Guid startId, Guid endId, VehicleType vehicleType, decimal price, string materials, double volume) : Order(clientId, startId, endId, vehicleType, price)
    {
        public string Materials { get; set; } = materials;
        public double Volume { get; set; } = volume;
    }
}
