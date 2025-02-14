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

namespace WinePOSFinal
{
    /// <summary>
    /// Interaction logic for InventoryMaintenanceWindow.xaml
    /// </summary>
    public partial class InventoryMaintenanceWindow : Window
    {
        public InventoryMaintenanceWindow()
        {
            InitializeComponent(); 
            inventoryControl.btnClear_Click(null, null); // Reset fields
        }

        public InventoryMaintenanceWindow(int itemId)
        {
            InitializeComponent();
            inventoryControl.PopulateData(itemId); // Call PopulateData on the UserControl
        }
    }
}
