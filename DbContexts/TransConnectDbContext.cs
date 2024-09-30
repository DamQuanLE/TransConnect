using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransConnect.Models;

namespace TransConnect.DbContexts
{
    public class TransConnectDbContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarOrder> CarOrders { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<DumpTruck> DumpTrucks { get; set; }
        public DbSet<Edge> Edges { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<HeavyTruck> HeavyTrucks { get; set; }
        public DbSet<HeavyTruckOrder> HeavyTruckOrders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<RefrigeratedTruck> RefrigeratedTrucks { get; set; }
        public DbSet<TankerTruck> TankerTrucks { get; set; }
        public DbSet<Van> Vans { get; set; }
        public DbSet<VanOrder> VanOrders { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<PriceList> PriceList { get; set; }
        public DbSet<OrderPath> OrderPaths { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(ConfigurationManager.ConnectionStrings["TransConnectDatabase"].ConnectionString);
            optionsBuilder.UseLazyLoadingProxies();
        }


        private class CSVEdge
        {
            public string Start { get; set; }
            public string End { get; set; }
            public double Distance { get; set; }
            public string Time { get; set; }
        }

        private class CSVEmployee
        {
            public int Stt { get; set; }
            public string Ssn { get; set; }
            public string Gender { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string DOB { get; set; }
            public string Address { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string DOE { get; set; }
            public string Position { get; set; }
            public decimal Salary { get; set; }
            public int Manager { get; set; }
        }

        private void seedEdges(ModelBuilder modelBuilder)
        {
            string seedFile = ConfigurationManager.AppSettings["EdgesCSV"]!;
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Delimiter = ","
            };

            Dictionary<string, Point> points = new Dictionary<string, Point>();
            Dictionary<Tuple<Point, Point>, Tuple<double, int>> edges = new Dictionary<Tuple<Point, Point>, Tuple<double, int>>();

            // string "1h20" to int 80, "35mn" to 35, "1h" to 60,...
            var timeParser = new Func<string, int>(s =>
            {
                int time = 0;
                if (s.Contains("h"))
                {
                    var tokens = s.Split('h');
                    if (tokens.Length == 2)
                    {
                        time += int.Parse(tokens[0]) * 60;
                        if (tokens[1].Length > 0)
                            time += int.Parse(tokens[1]);
                    }
                    else
                        time += int.Parse(s.Split('h')[0]) * 60;
                }
                else
                {
                    time += int.Parse(s.Split("mn")[0]);
                }
                return time;
            });

            using (var reader = new StreamReader(seedFile))
            {
                using (var csv = new CsvReader(reader, configuration))
                {
                    var records = csv.GetRecords<CSVEdge>();
                    foreach (var record in records)
                    {
                        // Add 2 points to the dictionary (if they don't exist)
                        Point p1;
                        if (points.ContainsKey(record.Start))
                            p1 = points[record.Start];
                        else
                        {
                            p1 = new Point(record.Start);
                            points.Add(record.Start, p1);
                        }

                        Point p2;
                        if (points.ContainsKey(record.End))
                            p2 = points[record.End];
                        else
                        {
                            p2 = new Point(record.End);
                            points.Add(record.End, p2);
                        }
                        // Add the edge to the dictionary
                        edges.Add(new Tuple<Point, Point>(p1, p2), new Tuple<double, int>(record.Distance, timeParser(record.Time)));
                    }
                }
            }

            for (int i = 0; i < points.Count; i++)
            {
                modelBuilder.Entity<Point>().HasData(points.ElementAt(i).Value);
            }

            foreach (var edge in edges)
            {
                modelBuilder.Entity<Edge>().HasData(new Edge(edge.Key.Item1.Id, edge.Key.Item2.Id, edge.Value.Item1, edge.Value.Item2));
            }
        }

        private void seedEmployees(ModelBuilder modelBuilder)
        {
            string seedFile = ConfigurationManager.AppSettings["EmployeeCSV"]!;
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Delimiter = ","
            };
            Dictionary<int, Guid> guids = new Dictionary<int, Guid>();
            Dictionary<Guid, Employee> employees = new Dictionary<Guid, Employee>();

            // Mapping gender string to enum
            var genderToEnum = new Func<string, PersonGender>(s =>
            {
                if (s == "Male")
                {
                    return PersonGender.MALE;
                }
                else
                {
                    return PersonGender.FEMALE;
                }
            });

            var stringDDMMYYYYToDateTime = new Func<string, DateTime>(s =>
            {
                var tokens = s.Split('/');
                if (tokens.Length == 3)
                {
                    return new DateTime(int.Parse(tokens[2]), int.Parse(tokens[1]), int.Parse(tokens[0]));
                }
                return new DateTime();
            });

            using (var reader = new StreamReader(seedFile))
            {
                using (var csv = new CsvReader(reader, configuration))
                {
                    var records = csv.GetRecords<CSVEmployee>();
                    foreach (var record in records)
                    {
                        // Create the employee and add it to the dictionary
                        var e = new Employee(record.Ssn, genderToEnum(record.Gender), record.LastName, record.FirstName, stringDDMMYYYYToDateTime(record.DOB), record.Address, record.Email, record.Phone, stringDDMMYYYYToDateTime(record.DOE), record.Position, record.Salary);
                        guids.Add(record.Stt, e.Id);
                        employees.Add(e.Id, e);
                        // Find the manager and set it
                        if (record.Manager != 0)
                        {
                            var g = guids[record.Manager];
                            e.ManagerId = g;
                        }
                    }
                }
            }

            for (int i = 0; i < employees.Count; i++)
            {
                modelBuilder.Entity<Employee>().HasData(employees.ElementAt(i).Value);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Point>()
                .HasMany(p => p.IncludedEdgesFromStartPoint)
                .WithOne(e => e.Start)
                .HasForeignKey(e => e.StartId)
                .IsRequired();

            modelBuilder.Entity<Point>()
                .HasMany(p => p.IncludedEdgesToEndPoint)
                .WithOne(e => e.End)
                .HasForeignKey(e => e.EndId)
                .IsRequired();


            seedEdges(modelBuilder);
            seedEmployees(modelBuilder);

            // Seed the database with the price list
            modelBuilder.Entity<PriceList>().HasData(
                new PriceList(0, 10, VehicleType.CAR, PriceType.BY_DISTANCE),
                new PriceList(10, 8, VehicleType.CAR, PriceType.BY_DISTANCE),
                new PriceList(0, 15, VehicleType.VAN, PriceType.BY_DISTANCE),
                new PriceList(10, 12, VehicleType.VAN, PriceType.BY_DISTANCE),
                new PriceList(0, 20, VehicleType.TANKER_TRUCK, PriceType.BY_DISTANCE),
                new PriceList(10, 18, VehicleType.TANKER_TRUCK, PriceType.BY_DISTANCE),
                new PriceList(0, 25, VehicleType.DUMP_TRUCK, PriceType.BY_DISTANCE),
                new PriceList(10, 22, VehicleType.DUMP_TRUCK, PriceType.BY_DISTANCE),
                new PriceList(0, 30, VehicleType.REFRIGERATED_TRUCK, PriceType.BY_DISTANCE),
                new PriceList(10, 28, VehicleType.REFRIGERATED_TRUCK, PriceType.BY_DISTANCE),
                new PriceList(0, 5, VehicleType.CAR, PriceType.BY_TIME),
                new PriceList(10, 4, VehicleType.CAR, PriceType.BY_TIME),
                new PriceList(0, 7, VehicleType.VAN, PriceType.BY_TIME),
                new PriceList(10, 6, VehicleType.VAN, PriceType.BY_TIME),
                new PriceList(0, 10, VehicleType.TANKER_TRUCK, PriceType.BY_TIME),
                new PriceList(10, 9, VehicleType.TANKER_TRUCK, PriceType.BY_TIME),
                new PriceList(0, 12, VehicleType.DUMP_TRUCK, PriceType.BY_TIME),
                new PriceList(10, 11, VehicleType.DUMP_TRUCK, PriceType.BY_TIME),
                new PriceList(0, 15, VehicleType.REFRIGERATED_TRUCK, PriceType.BY_TIME),
                new PriceList(10, 14, VehicleType.REFRIGERATED_TRUCK, PriceType.BY_TIME));

            // Seed the database with the vehicles
            modelBuilder.Entity<Car>().HasData(new Car(0));
            modelBuilder.Entity<Van>().HasData(new Van());
            modelBuilder.Entity<TankerTruck>().HasData(new TankerTruck());
            modelBuilder.Entity<DumpTruck>().HasData(new DumpTruck());
            modelBuilder.Entity<RefrigeratedTruck>().HasData(new RefrigeratedTruck());
        }

    }
}
