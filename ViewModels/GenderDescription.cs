using TransConnect.Models;

namespace TransConnect.ViewModels
{
    public class GenderDescription
    {
        public PersonGender PersonGender { get; set; }
        public string Description { get; set; }

        public static readonly List<GenderDescription> GenderDescriptions = new List<GenderDescription>
        {
            new GenderDescription { PersonGender = PersonGender.MALE, Description = "Male"},
            new GenderDescription { PersonGender = PersonGender.FEMALE, Description = "Female" },
        };

        public override string ToString()
        {
            return Description;
        }
    }
}
