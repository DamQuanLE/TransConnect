using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TransConnect.DbContexts;
using TransConnect.Models;

namespace TransConnect.ViewModels
{
    public class ClientManagementViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public BindingList<Client> Clients { get; set; } = new BindingList<Client>();


        private Client _selectedClient;

        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedClient"));

                if (value != null)
                {
                    SelectedGenderDescription = GenderDescriptions.First(gd => gd.PersonGender == value.Gender);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedGenderDescription"));

                    using (var db = new TransConnectDbContext())
                    {
                        Orders = new BindingList<Order>(db.Orders.Where(o => o.Client.Id == value.Id).OrderByDescending(o => o.CreationDate).ToList());
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Orders"));
                    }
                }
            }
        }

        public List<GenderDescription> GenderDescriptions { get; set; } = GenderDescription.GenderDescriptions;

        public GenderDescription SelectedGenderDescription { get; set; }

        public BindingList<Order> Orders { get; set; } = new BindingList<Order>();

        public ClientManagementViewModel()
        {
            loadClients();
            if (Clients.Count > 0)
            {
                SelectedClient = Clients.First();
            }
            else
            {
                prepareNewClient();
            }
        }


        private void prepareNewClient()
        {
            SelectedClient = new Client("", PersonGender.MALE, "", "", DateTime.Now, "", "", "");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedClient"));
        }


        private void loadClients()
        {
            using (var db = new TransConnectDbContext())
            {
                Clients = new BindingList<Client>(db.Clients.OrderBy(c => c.FirstName).ToList());
            }
        }

        public ICommand SaveClient => new RelayCommand(_ =>
        {
            if (SelectedClient == null)
            {
                return;
            }

            if (SelectedClient.FirstName == "" || SelectedClient.LastName == "" || SelectedClient.Phone == "" || SelectedClient.Email == "" || SelectedClient.Address == "" || SelectedGenderDescription == null)
            {
                MessageBox.Show("Please fill in all fields");
                return;
            }

            SelectedClient.Gender = SelectedGenderDescription.PersonGender;

            using (var db = new TransConnectDbContext())
            {
                if (Clients.Any(Clients => Clients.Id == SelectedClient.Id))
                {
                    db.Clients.Update(SelectedClient);
                    MessageBox.Show("Client updated successfully");
                }
                else
                {
                    db.Clients.Add(SelectedClient);
                    MessageBox.Show("Client added successfully");
                }

                db.SaveChanges();
                loadClients();
                SelectedClient = Clients.First(c => c.Id == SelectedClient.Id);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Clients"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedClient"));
            }
        });

        public ICommand SwitchToAddNewClientCommand => new RelayCommand(_ => prepareNewClient());

        public ICommand DeleteClient => new RelayCommand(_ =>
        {
            if (SelectedClient == null || Clients.Any(c => c.Id == SelectedClient.Id) == false)
            {
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this client?", "Delete Client", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            using (var db = new TransConnectDbContext())
            {
                // If Client has orders, do not delete
                if (db.Orders.Any(o => o.Client.Id == SelectedClient.Id))
                {
                    MessageBox.Show("Cannot delete because there are some orders assigned to this client");
                    return;
                }

                db.Clients.Remove(SelectedClient);
                db.SaveChanges();
                loadClients();
                
                if (Clients.Count > 0)
                {
                    SelectedClient = Clients.First();
                }
                else
                {
                    prepareNewClient();
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Clients"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedClient"));
            }
        });
    }
}
