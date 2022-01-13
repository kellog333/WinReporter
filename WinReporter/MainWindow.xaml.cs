using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
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
            this.Height = SystemParameters.PrimaryScreenHeight * 0.55;
            this.Width = SystemParameters.PrimaryScreenWidth * 0.600;
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
            TotalMemoryData.Content = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory").Get().Cast<ManagementObject>().Sum(x => Convert.ToInt64(x.Properties["Capacity"].Value) / 1024 / 1024 / 1024) + " GB";
            DataTable storageTable = new DataTable("StorageTable");
            var storageGridView = new GridView();
            StorageDriveList.View = storageGridView;
            storageGridView.Columns.Add(new GridViewColumn
            {
                Header = "Drive",
                DisplayMemberBinding = new Binding("Drive"),
                Width = 100
            });
            storageGridView.Columns.Add(new GridViewColumn
            {
                Header = "Available Space",
                DisplayMemberBinding = new Binding("Available"),
                Width = 140
            });
            storageGridView.Columns.Add(new GridViewColumn
            {
                Header = "Total Space",
                DisplayMemberBinding = new Binding("Total"),
                Width = 140
            });
            storageGridView.Columns.Add(new GridViewColumn
            {
                Header = "% Used",
                DisplayMemberBinding = new Binding("Used"),
                Width = 95
            });
            var opticalGridView = new GridView();
            OpticalDrivesList.View = opticalGridView;
            opticalGridView.Columns.Add(new GridViewColumn
            {
                Header = "Drive",
                DisplayMemberBinding = new Binding("Drive"),
                Width = 250
            });
            opticalGridView.Columns.Add(new GridViewColumn
            {
                Header = "Name",
                DisplayMemberBinding = new Binding("Name"),
                Width = 250
            });
            LoadStorageData();
            var memoryGridView = new GridView();
            MemoryList.View = memoryGridView;
            memoryGridView.Columns.Add(new GridViewColumn
            {
                Header = "Location",
                DisplayMemberBinding = new Binding("Location"),
                Width = 200
            });
            memoryGridView.Columns.Add(new GridViewColumn
            {
                Header = "Manufacturer",
                DisplayMemberBinding = new Binding("Manufacturer"),
                Width = 200
            });
            memoryGridView.Columns.Add(new GridViewColumn
            {
                Header = "Part Number",
                DisplayMemberBinding = new Binding("PartNumber"),
                Width = 200
            });
            memoryGridView.Columns.Add(new GridViewColumn
            {
                Header = "Capacity",
                DisplayMemberBinding = new Binding("Capacity"),
                Width = 200
            });
            GenerateMemoryInfo();
            var networkGridView = new GridView();
            NetworkList.View = networkGridView;
            networkGridView.Columns.Add(new GridViewColumn { Header = "Adapter", Width = 200, DisplayMemberBinding = new Binding("Adapter") });
            networkGridView.Columns.Add(new GridViewColumn { Header = "Type", Width = 150, DisplayMemberBinding = new Binding("Type") });
            networkGridView.Columns.Add(new GridViewColumn { Header = "MAC Address", Width = 150, DisplayMemberBinding = new Binding("MacAddress") });
            GenerateNetworkAdapters();
            FileLocation.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + MachineNameLabel.Content.ToString() + "_" + DateTime.Today.ToString("d").Replace("/","-") + ".pdf";
            GraphicsCardData.Content = LoadVideoData("Name");
            VideoRamData.Content = LoadVideoData("AdapterRAM");
            RefreshRateData.Content = LoadVideoData("CurrentRefreshRate");
            ResolutionData.Content = LoadVideoData("CurrentVerticalResolution") + " x " + LoadVideoData("CurrentHorizontalResolution");
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

        public void LoadStorageData()
        {
            DriveInfo[] driveInfos = DriveInfo.GetDrives();
            foreach (DriveInfo driveInfo in driveInfos)
            {
                if (driveInfo.IsReady == true && driveInfo.DriveType.ToString() == "Fixed")
                {
                    var availableSpace = driveInfo.AvailableFreeSpace / 1024 / 1024 / 1024;
                    var totalSpace = driveInfo.TotalSize / 1024 / 1024 / 1024;
                    var percentAvail = Math.Round((double)(100 * availableSpace) / totalSpace);
                    StorageDriveList.Items.Add(new ListItemsObjs.DriveItem { Available = availableSpace.ToString(), Drive = driveInfo.RootDirectory.ToString(), Total = totalSpace.ToString(), Used = percentAvail.ToString() });
                }
                if (driveInfo.DriveType.ToString() == "CDRom")
                {
                    OpticalDrivesList.Items.Add(new ListItemsObjs.OpticalDriveItem { Drive = driveInfo.RootDirectory.ToString(), Name = "CD / DVD Drive" });
                }
            }
        }

        public void GenerateMemoryInfo()
        {
            string locator = "";
            string manufacturer = "";
            string partnumber = "";
            string capacity = "";
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
            ManagementObjectCollection moc = mos.Get();
            foreach (ManagementObject m in moc)
            {
                foreach (PropertyData pd in m.Properties)
                {
                    if (pd.Name == "DeviceLocator")
                    {
                        if (pd.Value != null)
                        {
                            locator = pd.Value.ToString();
                        } else
                        {
                            locator = "Unknown";
                        }
                    }
                    if (pd.Name == "Manufacturer")
                    {
                        if (pd.Value != null)
                        {
                            manufacturer = pd.Value.ToString();
                        } else
                        {
                            manufacturer = "Unknown";
                        }
                    }
                    if (pd.Name == "PartNumber")
                    {
                        if (pd.Value != null)
                        {
                            partnumber = pd.Value.ToString();
                        } else
                        {
                            partnumber = "Unknown";
                        }
                    }
                    if (pd.Name == "Capacity")
                    {
                        if (pd.Value != null)
                        {
                            capacity = (Convert.ToInt64(pd.Value) / 1024 / 1024 / 1024).ToString();
                        } else
                        {
                            capacity = "Unknown";
                        }
                    }
                }
                    MemoryList.Items.Add(new ListItemsObjs.MemoryItem { Capacity = capacity + " GB", Location = locator, Manufacturer = manufacturer, PartNumber = partnumber });
            }
        }

        public void GenerateNetworkAdapters()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.NetworkInterfaceType.ToString() != "Loopback")
                {
                    NetworkList.Items.Add(new ListItemsObjs.NetworkDevice { Adapter = networkInterface.Description.ToString(), Type = networkInterface.NetworkInterfaceType.ToString(), MacAddress = networkInterface.GetPhysicalAddress().ToString() }); 
                }
            }
        }

        public string LoadVideoData(string data)
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (ManagementObject obj in mos.Get())
            {
                return obj[data].ToString();
            }
            return null;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult mbResult = MessageBox.Show("WinReporter ©2022\nAuthor: Kevin Heuser\nIf you have issues or would like to contribute, please reach out at https://github.com/kellog333/winreporter", "Information");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TabController.SelectedIndex = 6;
        }

        private void FileLocationButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime today = DateTime.Today;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "pdf";
            saveFileDialog.FileName = MachineNameLabel.Content.ToString() + "_" + today.ToString("d").Replace("/","-");
            saveFileDialog.Filter = "Pdf Files (*.pdf)|*.pdf|Text file(*.txt)|*.txt";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, saveFileDialog.DefaultExt);
                FileLocation.Text = saveFileDialog.FileName;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DirectoryWindow win = new DirectoryWindow();
            win.Show();
        }
    }
}
