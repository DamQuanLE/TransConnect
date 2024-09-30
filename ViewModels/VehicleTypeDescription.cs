using TransConnect.Models;

namespace TransConnect.ViewModels
{
    public class VehicleTypeDescription
    {
        public VehicleType Type { get; set; }
        public string Description { get; set; }

        public VehicleTypeDescription(VehicleType type, string description)
        {
            Type = type;
            Description = description;
        }
        public override string ToString()
        {
            return Description;
        }

        public static readonly List<VehicleTypeDescription> VehicleTypeDescriptions = new List<VehicleTypeDescription>
        {
            new VehicleTypeDescription(VehicleType.CAR, "Car"),
            new VehicleTypeDescription(VehicleType.VAN, "Van"),
            new VehicleTypeDescription(VehicleType.TANKER_TRUCK, "Tanker Truck"),
            new VehicleTypeDescription(VehicleType.DUMP_TRUCK, "Dump Truck"),
            new VehicleTypeDescription(VehicleType.REFRIGERATED_TRUCK, "Refrigerated Truck")
        };
    }
}
