using Microsoft.Win32;
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
            ManufacturerData.Content = GetManModel("Manufacturer");
            ModelNumberData.Content = GetManModel("Model");
            CoreCountData.Content = GetProcessorData("NumberOfCores");
            SerialNumberData.Content = GetSerialNumber();
            VersionData.Content = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows NT\CurrentVersion", "ProductName", null).ToString() + " Build " + Environment.OSVersion.Version.Build.ToString();
            ProductIdData.Content = GetProductId();

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

        public string GetManModel(string data)
        {
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection mcoc = mc.GetInstances();
            if (mcoc.Count != 0)
            {
                foreach (ManagementObject m in mcoc)
                {
                    return m[data].ToString();
                }
            }
            return null;
        }

        public string GetSerialNumber()
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard");
            foreach (ManagementObject m in mos.Get())
            {
                if (m["SerialNumber"].ToString().Length > 5)
                {
                    return m["SerialNumber"].ToString();
                } else
                {
                    return "Not Available";
                }
            }
            return "Not Available";
        }

        public string GetProductId()
        {
            ManagementObjectSearcher moc = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_OperatingSystem");
            ManagementObjectCollection mc = moc.Get();
            foreach (ManagementObject m in mc)
            {
                foreach (PropertyData data in m.Properties)
                {
                    if (data.Name == "SerialNumber")
                    {
                        return data.Value.ToString();
                    }
                }
            }
            return string.Empty;
        }
    }
}
