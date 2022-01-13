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

namespace WinReporter
{
    /// <summary>
    /// Interaction logic for DirectoryWindow.xaml
    /// </summary>
    public partial class DirectoryWindow : Window
    {
        public DirectoryWindow()
        {
            InitializeComponent();
            AddDirButtons();
        }

        public void AddDirButtons()
        {
            int buttonWidth = 150;
            int buttonHeight = 80;
            int j = 1;
            var items = ((MainWindow)Application.Current.MainWindow).StorageDriveList.Items;
            Random r = new Random();
            List<Helpers.ButtonPosition> buttonPositions = new List<Helpers.ButtonPosition>();
            foreach(ListItemsObjs.DriveItem i in items)
            {
                Button btn = new Button();
                btn.Content = i.Drive.ToString();
                btn.Width = buttonWidth;
                btn.Height = buttonHeight;
                btn.HorizontalAlignment = HorizontalAlignment.Left;
                var bc = new BrushConverter();
                btn.Background = (Brush)bc.ConvertFrom("#FF3A3838");
                btn.Foreground = Brushes.White;
                btn.FontSize = 15;
                btn.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                btn.Margin = new Thickness(5);
                Grid.SetRow(btn, j);
                MainGrid.Children.Add(btn);
                j++;
            }
        }
    }
}
