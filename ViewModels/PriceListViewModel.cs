using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TransConnect.DbContexts;
using TransConnect.Models;

namespace TransConnect.ViewModels
{
    public class PriceListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private BindingList<PriceList> _priceLists;

        public BindingList<PriceList> PriceLists
        {
            get { return _priceLists; }
            set
            {
                _priceLists = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PriceLists"));
            }
        }

        private PriceTypeDescription _selectedPriceType;

        public PriceTypeDescription SelectedPriceType
        {
            get { return _selectedPriceType; }
            set
            {
                _selectedPriceType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedPriceType"));
                if (SelectedVehicleType == null)
                {
                    return;
                }
                using (var context = new TransConnectDbContext())
                {
                    var priceLists = context.PriceList
                        .Where(p => p.PriceType == value.Type)
                        .Where(p => p.VehicleType == SelectedVehicleType.Type)
                        .OrderBy(p => p.From)
                        .ToList();
                    PriceLists = new BindingList<PriceList>(priceLists);
                }
            }
        }

        private VehicleTypeDescription _selectedVehicleType;
        public VehicleTypeDescription SelectedVehicleType
        {
            get { return _selectedVehicleType; }
            set
            {
                _selectedVehicleType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedVehicleType"));
                if (SelectedPriceType == null)
                {
                    return;
                }
                using (var context = new TransConnectDbContext())
                {
                    var priceLists = context.PriceList
                        .Where(p => p.PriceType == SelectedPriceType.Type)
                        .Where(p => p.VehicleType == value.Type)
                        .OrderBy(p => p.From)
                        .ToList();
                    PriceLists = new BindingList<PriceList>(priceLists);
                }
            }
        }

        public List<PriceTypeDescription> PriceTypes { get; set; }

        public List<VehicleTypeDescription> VehicleTypes { get; set; }

        public decimal PrepareFrom { get; set; } = 0M;

        public decimal PrepareUnitPrice { get; set; } = 0M;

        public int? SelectedIndex { get; set; }

        public PriceListViewModel()
        {
            PriceTypes = PriceTypeDescription.PriceTypeDescriptions;

            VehicleTypes = VehicleTypeDescription.VehicleTypeDescriptions;

            SelectedPriceType = PriceTypes[0];
            SelectedVehicleType = VehicleTypes[0];
        }

        private void handleSave()
        {
            using (var context = new TransConnectDbContext())
            {
                foreach (var priceList in PriceLists)
                {
                    context.PriceList.Update(priceList);
                }
                context.SaveChanges();
                MessageBox.Show("Saved");
            }
        }

        public ICommand SaveCommand => new RelayCommand((p) => handleSave());

        public ICommand AddCommand => new RelayCommand((_) =>
        {
            if (PrepareFrom > 0 && PrepareUnitPrice > 0)
            {
                // Check if the prepare from is already in the list
                if (PriceLists.Any(p => p.From == PrepareFrom))
                {
                    MessageBox.Show("The distance already exists");
                    return;
                }

                var priceList = new PriceList(PrepareFrom, PrepareUnitPrice, SelectedVehicleType.Type, SelectedPriceType.Type);
                PriceLists.Add(priceList);
                using (var context = new TransConnectDbContext())
                {
                    context.PriceList.Add(priceList);
                    context.SaveChanges();
                }
            }
            else
            {
                MessageBox.Show("Please enter price and unit price");
            }
        });

        public ICommand DeleteCommand => new RelayCommand((_) =>
        {
            if (SelectedIndex.HasValue)
            {
                if (SelectedIndex.Value < 0)
                {
                    MessageBox.Show("Please select an item to delete");
                    return;
                }
                if (SelectedIndex.Value == 0)
                {
                    MessageBox.Show("You cannot delete the first item");
                    return;
                }
                if (MessageBox.Show("Are you sure you want to delete this item?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
                var priceList = PriceLists[SelectedIndex.Value];
                PriceLists.RemoveAt(SelectedIndex.Value);
                using (var context = new TransConnectDbContext())
                {
                    context.PriceList.Remove(priceList);
                    context.SaveChanges();
                }
            }
        });


    }
}
