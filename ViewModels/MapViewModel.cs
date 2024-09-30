using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TransConnect.Algos;
using TransConnect.DbContexts;

namespace TransConnect.ViewModels
{
    public class MapViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public BindingList<Models.Point> Points { get; set; }

        private Models.Point _selectedStartPoint;
        public Models.Point SelectedStartPoint
        {
            get { return _selectedStartPoint; }
            set
            {
                if (_selectedStartPoint == value) return;
                _selectedStartPoint = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedStartPoint"));
                fetchAllDestPoints();
                if (EndPoints.Count > 0)
                {
                    SelectedEndPoint = EndPoints.First();
                } else
                {
                    SelectedEndPoint = null;
                    Distance = 0;
                    Time = 0;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Distance"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Time"));
                }
                fetchAllUnDestPoints();
            }
        }

        public double Distance { get; set; } = 0;

        public int Time { get; set; } = 0;

        public BindingList<Models.Point> EndPoints { get; set; }

        private Models.Point _selectedEndPoint;
        public Models.Point SelectedEndPoint
        {
            get { return _selectedEndPoint; }
            set
            {
                _selectedEndPoint = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedEndPoint"));
                if (value == null) return;

                using (var context = new TransConnectDbContext())
                {
                    var edge = context.Edges
                        .FirstOrDefault(e => (e.StartId == SelectedStartPoint.Id && e.EndId == value.Id) || (e.StartId == value.Id && e.EndId == SelectedStartPoint.Id));
                    if (edge != null)
                    {
                        Distance = edge.Distance;
                        Time = edge.Time;

                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Distance"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Time"));
                    }
                }
            }
        }

        public BindingList<Models.Point> UnEndPoints { get; set; }
        public Models.Point SelectedUnEndPoint { get; set; }

        public Models.Point SelectedRoutingStart { get; set; }
        public Models.Point SelectedRoutingEnd { get; set; }
        public PriceTypeDescription SelectedPriceType { get; set; }
        public BindingList<Models.Point> Routes { get; set; }

        public string PreparePointName { get; set; }

        public double PrepareDistance { get; set; } = 0;

        public int PrepareTime { get; set; } = 0;

        public MapViewModel()
        {
            fetchAllPoints();
            if (Points.Count > 0)
            {
                SelectedStartPoint = Points.First();
            }
        }

        private void updateEdgeHandler()
        {
            if (SelectedStartPoint == null || SelectedEndPoint == null || Distance <= 0 || Time <= 0)
            {
                MessageBox.Show("Invalid start/end points or distance/time");
                return;
            }

            using (var context = new TransConnectDbContext())
            {
                var edge = context.Edges
                    .FirstOrDefault(e => (e.StartId == SelectedStartPoint.Id && e.EndId == SelectedEndPoint.Id) || (e.StartId == SelectedEndPoint.Id && e.EndId == SelectedStartPoint.Id));
                if (edge != null)
                {
                    edge.Distance = Distance;
                    edge.Time = Time;
                    context.SaveChanges();
                }
                MessageBox.Show("Edge updated successfully");
            }
        }

        private void findRouteHandler()
        {
            if (SelectedRoutingStart == null || SelectedRoutingEnd == null || SelectedRoutingStart.Id == SelectedRoutingEnd.Id)
            {
                MessageBox.Show("Invalid start/end points");
                return;
            }

            using (var context = new TransConnectDbContext())
            {
                var route = Routing.Dijkstra(SelectedRoutingStart, SelectedRoutingEnd);
                if (route != null)
                {
                    Routes = new BindingList<Models.Point>(route.Item2);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Routes"));
                }
                else
                {
                    MessageBox.Show("Route not found");
                    Routes.Clear();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Routes"));
                }
            }
        }

        public ICommand UpdateEdgeCommand => new RelayCommand((parameter) => updateEdgeHandler());
        public ICommand FindRouteCommand => new RelayCommand((parameter) => findRouteHandler());

        public ICommand AddNewPointCommand => new RelayCommand((parameter) =>
        {
            if (string.IsNullOrEmpty(PreparePointName))
            {
                MessageBox.Show("Invalid point name");
                return;
            }

            using (var context = new TransConnectDbContext())
            {
                var point = new Models.Point(PreparePointName);
                context.Points.Add(point);
                context.SaveChanges();
                MessageBox.Show("Point added successfully");
                fetchAllPoints();
                SelectedStartPoint = Points.First(p => p.Id == point.Id);
            }
        });

        public ICommand DeletePointCommand => new RelayCommand((parameter) =>
        {
            if (SelectedStartPoint == null)
            {
                MessageBox.Show("Invalid point");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this point?", "Delete Point", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            using (var context = new TransConnectDbContext())
            {
                var point = context.Points.FirstOrDefault(p => p.Id == SelectedStartPoint.Id);

                if (point == null) return;

                // Prevent deleting the point if it has an edge
                if (context.Edges.Any(e => e.StartId == SelectedStartPoint.Id || e.EndId == SelectedStartPoint.Id))
                {
                    MessageBox.Show("Point has an edge, cannot delete! Please delete the edge first");
                    return;
                }

                context.Points.Remove(point);

                context.SaveChanges();
                MessageBox.Show("Point deleted successfully");
                fetchAllPoints();
                if (Points.Count > 0)
                {
                    SelectedStartPoint = Points.First();
                } else
                {
                    SelectedStartPoint = null;

                    Distance = 0;
                    Time = 0;

                    PrepareDistance = 0;
                    PrepareTime = 0;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PrepareDistance"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PrepareTime"));

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Distance"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Time"));
                }
            }
        });

        public ICommand AddEdgeCommand => new RelayCommand((parameter) =>
        {
            if (SelectedStartPoint == null || SelectedUnEndPoint == null || PrepareDistance <= 0 || PrepareTime <= 0)
            {
                MessageBox.Show("Invalid start/end points or distance/time");
                return;
            }

            using (var context = new TransConnectDbContext())
            {
                var edge = new Models.Edge(SelectedStartPoint.Id, SelectedUnEndPoint.Id, PrepareDistance, PrepareTime);
                context.Edges.Add(edge);
                context.SaveChanges();
                MessageBox.Show("Edge added successfully");
                fetchAllDestPoints();
                SelectedEndPoint = EndPoints.First(p => p.Id == SelectedUnEndPoint.Id);

                fetchAllUnDestPoints();
                if (UnEndPoints.Count > 0)
                {
                    SelectedUnEndPoint = UnEndPoints.First();
                    PrepareDistance = 0;
                    PrepareTime = 0;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PrepareDistance"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PrepareTime"));
                } else
                {
                    SelectedUnEndPoint = null;
                }

                
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedUnEndPoint"));
            }
        });

        public ICommand DeleteEdgeCommand => new RelayCommand((parameter) =>
        {
            if (SelectedStartPoint == null || SelectedEndPoint == null)
            {
                MessageBox.Show("Invalid start/end points");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this edge?", "Delete Edge", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            using (var context = new TransConnectDbContext())
            {
                var edge = context.Edges.FirstOrDefault(e => (e.StartId == SelectedStartPoint.Id && e.EndId == SelectedEndPoint.Id) || (e.StartId == SelectedEndPoint.Id && e.EndId == SelectedStartPoint.Id));

                if (edge == null) return;

                context.Edges.Remove(edge);
                context.SaveChanges();
                MessageBox.Show("Edge deleted successfully");
                fetchAllDestPoints();
                if (EndPoints.Count > 0)
                {
                    SelectedEndPoint = EndPoints.First();
                } else
                {
                    SelectedEndPoint = null;
                    Distance = 0;
                    Time = 0;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Distance"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Time"));
                }

                fetchAllUnDestPoints();
                if (UnEndPoints.Count > 0)
                {
                    SelectedUnEndPoint = UnEndPoints.First();
                } else
                {
                    SelectedUnEndPoint = null;
                }
                
            }
        });

        private void fetchAllPoints()
        {
            using (var context = new TransConnectDbContext())
            {
                var points = context.Points.OrderBy(p => p.Name).ToList();
                Points = new BindingList<Models.Point>(points);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Points"));
            }
        }

        private void fetchAllDestPoints()
        {
            using (var context = new TransConnectDbContext())
            {
                // Show all points that have a connection to the selected end point via an edge (start/end points)
                var points = context.Edges
                    .Include(e => e.Start)
                    .Include(e => e.End)
                    .Where(e => e.StartId == SelectedStartPoint.Id || e.EndId == SelectedStartPoint.Id)
                    .Select(e => e.StartId == SelectedStartPoint.Id ? e.End : e.Start)
                    .ToList();
                points.Sort();
                EndPoints = new BindingList<Models.Point>(points);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EndPoints"));
            }
        }

        private void fetchAllUnDestPoints()
        {
            // Show all points that don't have a connection to the selected end point via an edge (start/end points)
            using (var context = new TransConnectDbContext())
            {
                var points = context.Points
                    .Where(p => p.Id != SelectedStartPoint.Id && !context.Edges.Any(e => e.StartId == SelectedStartPoint.Id && e.EndId == p.Id) && !context.Edges.Any(e => e.StartId == p.Id && e.EndId == SelectedStartPoint.Id))
                    .ToList();
                points.Sort();
                UnEndPoints = new BindingList<Models.Point>(points);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UnEndPoints"));
                if (UnEndPoints.Count > 0)
                {
                    SelectedUnEndPoint = UnEndPoints.First();
                } else
                {
                    SelectedUnEndPoint = null;
                }
                PrepareDistance = 0;
                PrepareTime = 0;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PrepareDistance"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PrepareTime"));

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedUnEndPoint"));
            }
        }
    }
}
