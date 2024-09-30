namespace TransConnect.Models
{
    public enum PriceType
    {
        BY_DISTANCE,
        BY_TIME
    }

    public class PriceList(decimal from, decimal unitPrice, VehicleType vehicleType, PriceType priceType)
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal From { get; set; } = from;
        public decimal UnitPrice { get; set; } = unitPrice;
        public VehicleType VehicleType { get; set; } = vehicleType;
        public PriceType PriceType { get; set; } = priceType;
    }
}