using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TransConnect.DbContexts;
using TransConnect.Models;

namespace TransConnect.ViewModels
{
    public class EmployeeManagementViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public BindingList<Employee> Employees { get; set; } = new BindingList<Employee>();

        private void loadEmployees()
        {
            using (var db = new TransConnectDbContext())
            {
                Employees = new BindingList<Employee>(db.Employees.Include(e => e.Manager).OrderBy(e => e.FirstName).ToList());
            }
        }

        private Employee _selectedEmployee;

        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedEmployee"));
                if (value == null)
                {
                    return;
                }
                SelectedGenderDescription = GenderDescriptions.First(gd => gd.PersonGender == value.Gender);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedGenderDescription"));
                if (value.Position == "Driver")
                {
                    IsDriver = true;
                    // Attach Order to Driver
                    using (var db = new TransConnectDbContext())
                    {
                        OrderCount = db.Orders.Count(o => o.Driver.Id == value.Id);
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OrderCount"));
                }
                else
                {
                    IsDriver = false;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DriverVisibility"));

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsDriver"));

                SelectedManager = value.Manager;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedManager"));

            }
        }

        public int OrderCount { get; set; }

        public List<GenderDescription> GenderDescriptions { get; set; } = GenderDescription.GenderDescriptions;

        public GenderDescription SelectedGenderDescription { get; set; }
        
        public bool IsDriver { get; set; }

        public Employee SelectedManager { get; set; }

        public EmployeeManagementViewModel()
        {
            loadEmployees();
            if (Employees.Count > 0)
            {
                SelectedEmployee = Employees.First();
            } else
            {
                prepareNewEmployee();
            }
        }

        public string DriverVisibility => IsDriver ? "Visible" : "Hidden";

        private void prepareNewEmployee()
        {
            SelectedEmployee = new Employee("", PersonGender.MALE, "", "", DateTime.Now, "", "", "", DateTime.Now, "", 0);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedEmployee"));
        }

        public ICommand SwitchToAddNewEmployeeCommand => new RelayCommand(_ => prepareNewEmployee());

        public ICommand SaveEmployee => new RelayCommand(_ =>
        {
            if (SelectedEmployee == null)
            {
                return;
            }

            if (SelectedEmployee.FirstName == "" || SelectedEmployee.LastName == "" || SelectedEmployee.Phone == "" || SelectedEmployee.Email == "" || SelectedEmployee.Address == "" || SelectedEmployee.Salary == 0 || SelectedGenderDescription == null)
            {
                MessageBox.Show("Please fill in all fields");
                return;
            }

            using (var db = new TransConnectDbContext())
            {
                if (IsDriver)
                {
                    SelectedEmployee.Position = "Driver";
                }
                if (SelectedManager != null)
                {
                    SelectedEmployee.ManagerId = SelectedManager.Id;
                }

                if (Employees.Any(Employees => Employees.Id == SelectedEmployee.Id))
                {
                    db.Employees.Update(SelectedEmployee);
                    MessageBox.Show("Employee updated successfully");
                }
                else
                {
                    db.Employees.Add(SelectedEmployee);
                    MessageBox.Show("Employee added successfully");
                }
                db.SaveChanges();
                loadEmployees();
                SelectedEmployee = Employees.First(e => e.Id == SelectedEmployee.Id);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Employees"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedEmployee"));
            }
        });
        
        public ICommand DeleteEmployee => new RelayCommand(_ =>
        {
            if (SelectedEmployee == null || Employees.Any(e => e.Id == SelectedEmployee.Id) == false)
            {
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete this employee?", "Delete Employee", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }
            using (var db = new TransConnectDbContext())
            {
                // If employee is a driver, cannot delete if there are orders assigned to the driver
                if (SelectedEmployee.Position == "Driver")
                {
                    var ordersAssignedToDriver = db.Orders.Where(o => o.Driver.Id == SelectedEmployee.Id).ToList();
                    if (ordersAssignedToDriver.Count > 0)
                    {
                        MessageBox.Show("Cannot delete because there are some orders assigned to this driver");
                        return;
                    }
                }
                
                // If employee is a manager, set all employees managed by this employee to null
                var employeesManagedBySelectedEmployee = db.Employees.Where(e => e.Manager.Id == SelectedEmployee.Id).ToList();
                foreach (var employee in employeesManagedBySelectedEmployee)
                {
                    employee.Manager = null;
                    db.Employees.Update(employee);
                }

                db.Employees.Remove(SelectedEmployee);
                db.SaveChanges();
                loadEmployees();
                if (Employees.Count > 0)
                {
                    SelectedEmployee = Employees.First();
                } else
                {
                    prepareNewEmployee();
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Employees"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedEmployee"));
            }
        });
    }
}
