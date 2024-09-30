using System.Collections.ObjectModel;

namespace TransConnect.Models
{
    public class Point(string name) : IComparable<Point>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public virtual ICollection<Edge> IncludedEdgesFromStartPoint { get; private set; } = new ObservableCollection<Edge>();
        public virtual ICollection<Edge> IncludedEdgesToEndPoint { get; private set; } = new ObservableCollection<Edge>();
        public string Name { get; set; } = name;

        public override bool Equals(object? obj)
        {
            return obj is Point point &&
                   Name == point.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public override string ToString()
        {
            return Name;
        }

        // implement comparison for sort() method
        public int CompareTo(Point other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
