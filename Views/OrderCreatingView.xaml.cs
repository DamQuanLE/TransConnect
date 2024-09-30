using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TransConnect.Models;
using TransConnect.ViewModels;

namespace TransConnect.Views
{
    /// <summary>
    /// Interaction logic for OrderCreatingView.xaml
    /// </summary>
    public partial class OrderCreatingView : Window
    {
        public OrderCreatingView()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = (ComboBox)sender;
            var selected = (VehicleTypeDescription)cmb.SelectedItem;
            if (selected == null)
            {
                return;
            }
            if (stkForCar == null || stkForVan == null || stkForHeavyTruck == null)
            {
                return;
            }

            if (selected.Type == VehicleType.CAR)
            {
                stkForCar.Visibility = Visibility.Visible;
                stkForVan.Visibility = Visibility.Collapsed;
                stkForHeavyTruck.Visibility = Visibility.Collapsed;
            }
            else if (selected.Type == VehicleType.VAN)
            {
                stkForCar.Visibility = Visibility.Collapsed;
                stkForVan.Visibility = Visibility.Visible;
                stkForHeavyTruck.Visibility = Visibility.Collapsed;
            }
            else
            {
                stkForCar.Visibility = Visibility.Collapsed;
                stkForVan.Visibility = Visibility.Collapsed;
                stkForHeavyTruck.Visibility = Visibility.Visible;
            }
        }
    }
}
