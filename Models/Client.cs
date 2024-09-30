using System.Collections.ObjectModel;

namespace TransConnect.Models
{
    public class Client(string socialSecurityNumber, PersonGender gender, string lastName, string firstName, DateTime dateOfBirth, string address, string email, string phone) : Person(socialSecurityNumber, gender, lastName, firstName, dateOfBirth, address, email, phone)
    {
        public virtual ICollection<Order> Orders { get; private set; } = new ObservableCollection<Order>();
    }
}