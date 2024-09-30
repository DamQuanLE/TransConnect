using System.Collections.ObjectModel;

namespace TransConnect.Models
{
    public class Employee(string socialSecurityNumber, PersonGender gender, string lastName, string firstName, DateTime dateOfBirth, string address, string email, string phone, DateTime dateOfEntry, string position, decimal salary) : Person(socialSecurityNumber, gender, lastName, firstName, dateOfBirth, address, email, phone)
    {
        public DateTime DateOfEntry { get; set; } = dateOfEntry;
        public string Position { get; set; } = position;
        public decimal Salary { get; set; } = salary;
        public Guid? ManagerId { get; set; }
        public virtual Employee? Manager { get; set;}

        public virtual ICollection<Order> Orders { get; private set; } = new ObservableCollection<Order>();

        public override string ToString()
        {
            return $"{base.ToString()} ({Position})";
        }
    }
}