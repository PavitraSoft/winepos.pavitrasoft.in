using System;
using System.Collections.Generic;
using System.Data;
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
using WinePOSFinal.Classes;
using WinePOSFinal.ServicesLayer;

namespace WinePOSFinal.UserControls
{
    /// <summary>
    /// Interaction logic for SalesHistory.xaml
    /// </summary>
    public partial class SalesHistory : UserControl
    {
        WinePOSService objService = new WinePOSService();
        public SalesHistory()
        {
            InitializeComponent();

            FetchAndPopulateInvoice();
        }

        private void FetchAndPopulateInvoice()
        {

           DataTable dtInvoice = objService.FetchAndPopulateInvoice(true);


            InventoryDataGrid.ItemsSource = dtInvoice.DefaultView;
        }
    }
}
