namespace TransConnect.Models
{
    public class Edge(Guid startId, Guid endId, double distance, int time)
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid StartId { get; set; } = startId;
        public virtual Point? Start { get; set; }
        public Guid EndId { get; set; } = endId;
        public virtual Point? End { get; set; }
        public double Distance { get; set; } = distance;
        public int Time { get; set; } = time;
    }
}
