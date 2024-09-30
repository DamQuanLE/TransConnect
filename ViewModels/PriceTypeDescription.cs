using TransConnect.Models;

namespace TransConnect.ViewModels
{
    public class PriceTypeDescription
    {
        public PriceType Type { get; set; }
        public string Description { get; set; }

        public PriceTypeDescription(PriceType type, string description)
        {
            Type = type;
            Description = description;
        }
        public override string ToString()
        {
            return Description;
        }

        public static readonly List<PriceTypeDescription> PriceTypeDescriptions = new List<PriceTypeDescription>
        {
            new PriceTypeDescription(PriceType.BY_TIME, "By Time"),
            new PriceTypeDescription(PriceType.BY_DISTANCE, "By Distance")
        };
    }
}
