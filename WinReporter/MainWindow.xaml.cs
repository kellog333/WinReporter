using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
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

namespace WinReporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TabControl_Selected(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(sender.ToString());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MachineNameLabel.Content = Environment.MachineName.ToString();
            ProcessorData.Content = GetProcessorData("name");
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public string GetProcessorData(string data)
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM WIN32_Processor");
            ManagementObjectCollection moc = mos.Get();
            foreach(ManagementObject m in moc)
            {
                return m[data].ToString();
            }
            return null;
        }
    }
}
