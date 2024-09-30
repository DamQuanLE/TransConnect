using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TransConnect.Models;
using System.Reflection.Emit;
using System.Diagnostics;
using TransConnect.Views;

namespace TransConnect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        // You only open one window at a time (except for the main window)
        private Window? _currentWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOrderMgt_Click(object sender, RoutedEventArgs e)
        {
            if (_currentWindow != null)
            {
                _currentWindow.Close();
            }
            _currentWindow = new OrderManagementView();
            _currentWindow.Show();

        }

        private void btnEmplMgt_Click(object sender, RoutedEventArgs e)
        {
            if (_currentWindow != null)
            {
                _currentWindow.Close();
            }
            _currentWindow = new EmployeeView();
            _currentWindow.Show();
        }

        private void btnMap_Click(object sender, RoutedEventArgs e)
        {
            if (_currentWindow != null)
            {
                _currentWindow.Close();
            }
            _currentWindow = new MapView();
            _currentWindow.Show();
        }

        private void btnEmplTV_Click(object sender, RoutedEventArgs e)
        {
            if (_currentWindow != null)
            {
                _currentWindow.Close();
            }
            _currentWindow = new EmployeeTreeView();
            _currentWindow.Show();
        }

        private void btnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_currentWindow != null)
            {
                _currentWindow.Close();
            }
            _currentWindow = new OrderCreatingView();
            _currentWindow.Show();
        }

        private void btnPriceList_Click(object sender, RoutedEventArgs e)
        {
            if (_currentWindow != null)
            {
                _currentWindow.Close();
            }
            _currentWindow = new PriceListView();
            _currentWindow.Show();
        }

        private void btnClientMgt_Click(object sender, RoutedEventArgs e)
        {
            if (_currentWindow != null)
            {
                _currentWindow.Close();
            }
            _currentWindow = new ClientManagementView();
            _currentWindow.Show();
        }

        private void btnOrderStats_Click(object sender, RoutedEventArgs e)
        {
            if (_currentWindow != null)
            {
                _currentWindow.Close();
            }
            _currentWindow = new OrderStatsView();
            _currentWindow.Show();
        }

        private void btnClientRk_Click(object sender, RoutedEventArgs e)
        {
            if (_currentWindow != null)
            {
                _currentWindow.Close();
            }
            _currentWindow = new ClientRankingView();
            _currentWindow.Show();
        }
    }
}