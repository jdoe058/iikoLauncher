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
using System.Windows.Navigation;
using System.Windows.Shapes;
using iikoLauncher.Models;
using System.Xml;

namespace iikoLauncher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is XmlElement server)) return;
            
            var filter = ServersFilter.Text;
            if (filter.Length == 0) return;

            if (server?.Attributes["Name"]?.Value.IndexOf(filter, StringComparison.OrdinalIgnoreCase) > -1) return;
            if (server?.Attributes["ChainName"]?.Value.IndexOf(filter, StringComparison.OrdinalIgnoreCase) > -1) return;

            e.Accepted = false;
        }

        private void ServersFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            var cvs = (CollectionViewSource)Resources["cvs"];
            cvs.View.Refresh();
        }
    }
}
