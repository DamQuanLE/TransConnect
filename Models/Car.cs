namespace TransConnect.Models
{
    public class Car(int seats) : Vehicle
    {
        public int Seats { get; set; } = seats;
    }
}