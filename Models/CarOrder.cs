namespace TransConnect.Models
{
    public class CarOrder(Guid clientId, Guid startId, Guid endId, decimal price, int numOfPassengers) : Order(clientId, startId, endId, VehicleType.CAR, price)
    {
        public int NumOfPassengers { get; set; } = numOfPassengers;
    }
}
