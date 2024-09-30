using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransConnect.DbContexts;
using TransConnect.Models;

namespace TransConnect.Algos
{
    public static class Routing
    {
        public static Tuple<double, List<Point>>? Dijkstra(Point start, Point end, PriceType distanceType = PriceType.BY_DISTANCE)
        {
            Dictionary<Point, double> distances = new Dictionary<Point, double>();
            Dictionary<Point, Point> previous = new Dictionary<Point, Point>();

            List<Edge> edges = new List<Edge>();
            using (var context = new TransConnectDbContext())
            {
                edges = context.Edges.Include("Start").Include("End").ToList();
            }
            List<Point> nodes = new List<Point>();

            // Neighbors
            Dictionary<Point, List<Tuple<Point, double>>> neighbors = new Dictionary<Point, List<Tuple<Point, double>>>();
            List<Point> points = new List<Point>();
            using (var context = new TransConnectDbContext())
            {
                points = context.Points.ToList();
            }
            foreach (var point in points)
            {
                neighbors[point] = new List<Tuple<Point, double>>();
            }
            foreach (var edge in edges)
            {
                if (distanceType == PriceType.BY_DISTANCE)
                {
                    neighbors[edge.Start].Add(new Tuple<Point, double>(edge.End, edge.Distance));
                    neighbors[edge.End].Add(new Tuple<Point, double>(edge.Start, edge.Distance));
                }
                else
                {
                    neighbors[edge.Start].Add(new Tuple<Point, double>(edge.End, edge.Time));
                    neighbors[edge.End].Add(new Tuple<Point, double>(edge.Start, edge.Time));
                }
            }

            // Initialize the distances
            foreach (Point point in points)
            {
                if (point.Equals(start))
                {
                    distances[point] = 0;
                }
                else
                {
                    distances[point] = double.MaxValue;
                }
                nodes.Add(point);
            }


            // Visited
            HashSet<Point> visited = new HashSet<Point>();


            while (nodes.Count != 0)
            {
                // Pick the smallest distance node
                nodes.Sort((x, y) => distances[x] < distances[y] ? -1 : 1);
                Point smallest = nodes[0];
                nodes.Remove(smallest);

                // No ways
                if (Math.Abs(distances[smallest] - double.MaxValue) < 1.0)
                {
                    return null;
                }

                // Success
                if (smallest.Equals(end))
                {
                    List<Point> path = new List<Point>();
                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(smallest);
                        smallest = previous[smallest];
                    }

                    path.Add(start);
                    path.Reverse();
                    // Shortest distance
                    var shortestDistance = distances[end];

                    return new Tuple<double, List<Point>>(shortestDistance, path);
                }

                // Update distances
                foreach (var neighbor in neighbors[smallest])
                {
                    double alt = distances[smallest] + neighbor.Item2;
                    if (alt < distances[neighbor.Item1])
                    {
                        distances[neighbor.Item1] = alt;
                        previous[neighbor.Item1] = smallest;
                    }
                }
            }

            return null;
        }
    }
}
