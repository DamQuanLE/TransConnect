namespace TransConnect.Models
{
    public class OrderPath(Guid orderId, Guid fromId, Guid toId, int number)
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; } = orderId;
        public virtual Order? Order { get; set; }
        public Guid FromId { get; set; } = fromId;
        public virtual Point? From { get; set; }
        public Guid ToId { get; set; } = toId;
        public virtual Point? To { get; set; }
        public int Number { get; set; } = number;

        public override string ToString()
        {
            return $"{From} to {To}";
        }
    }
}