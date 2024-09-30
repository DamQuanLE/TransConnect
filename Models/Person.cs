namespace TransConnect.Models
{
    public enum PersonGender
    {
        MALE,
        FEMALE
    }

    public class Person(string socialSecurityNumber, PersonGender gender, string lastName, string firstName, DateTime dateOfBirth, string address, string email, string phone)
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string SocialSecurityNumber { get; set; } = socialSecurityNumber;
        public PersonGender Gender { get; set; } = gender;
        public string LastName { get; set; } = lastName;
        public string FirstName { get; set; } = firstName;
        public DateTime DateOfBirth { get; set; } = dateOfBirth;
        public string Address { get; set; } = address;
        public string Email { get; set; } = email;
        public string Phone { get; set; } = phone;
        public string FullName => $"{FirstName} {LastName}";

        public override string ToString()
        {
            if (Gender == PersonGender.MALE)
            {
                return $"Mr. {FullName}";
            }
            else
            {
                return $"Ms. {FullName}";
            }
        }
    }

}