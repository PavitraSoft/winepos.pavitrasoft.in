using Microsoft.PointOfService;
using System;
using System.Collections.Generic;
using System.IO;
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
using WinePOSFinal.UserControls;

namespace WinePOSFinal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        private PosExplorer explorer;

        public CashDrawer cashDrawer;

        public PosPrinter m_Printer = null;


        public MainWindow()
        {
            InitializeComponent();


            InitializeCashDrawer();
            InitializePrinter();

            string currentRole = AccessRightsManager.GetUserRole();

            Inventorymaintenance.Visibility = Visibility.Collapsed;

            if (currentRole.ToUpper() == "ADMIN")
            {
                Inventorymaintenance.Visibility = Visibility.Visible;
            }
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //// Check which tab is selected and reload data accordingly
            //if (Billing.IsSelected)
            //{
            //    // Ensure the Billing content is initialized before trying to access it
            //    if (Billing.Content is Billing billingControl)
            //    {
            //        billingControl.ReloadBillingData(); // Reload Billing data
            //    }
            //}
            //else if (SearchInventory.IsSelected)
            //{
            //    // Ensure the SearchInventory content is initialized before trying to access it
            //    if (SearchInventory.Content is SearchInventory searchInventoryControl)
            //    {
            //        searchInventoryControl.ReloadSearchInventoryData(); // Reload Search Inventory data
            //    }
            //}
            //else if (Inventorymaintenance.IsSelected)
            //{
            //    // Ensure the Inventorymaintenance content is initialized before trying to access it
            //    if (Inventorymaintenance.Content is InventoryMaintenance inventoryMaintenanceControl)
            //    {
            //        inventoryMaintenanceControl.ReloadInventoryMaintenanceData(); // Reload Inventory Maintenance data
            //    }
            //}
            //else if (SalesHistory.IsSelected)
            //{
            //    // Ensure the SalesHistory content is initialized before trying to access it
            //    if (SalesHistory.Content is SalesHistory salesHistoryControl)
            //    {
            //        salesHistoryControl.ReloadSalesHistoryData(); // Reload Sales History data
            //    }
            //}
        }

        private void InitializeCashDrawer()

        {

            try

            {

                explorer = new PosExplorer();
                string strLogicalName = "CashDrawer";

                DeviceInfo deviceInfo = explorer.GetDevice(DeviceType.CashDrawer, strLogicalName);

                cashDrawer = (CashDrawer)explorer.CreateInstance(deviceInfo);

                cashDrawer.Open();
                cashDrawer.Claim(1000);

                cashDrawer.DeviceEnabled = true;

            }

            catch (Exception ex)

            {

                MessageBox.Show("Error initializing cash drawer: " + ex.Message);

            }

        }

        private void InitializeCashDrawer(bool use)

        {

            //<<<step1>>>--Start
            //Use a Logical Device Name which has been set on the SetupPOS.
            string strLogicalName = "CashDrawer";

            //Create PosExplorer
            PosExplorer posExplorer = new PosExplorer();

            DeviceInfo deviceInfo = null;

            //<<<step3>>>--Start
            try
            {
                deviceInfo = posExplorer.GetDevice(DeviceType.CashDrawer, strLogicalName);
            }
            catch (Exception)
            {
                //MessageBox.Show("Failed to get device information.", MessageBoxButton.OK, MessageBoxImage.Information);
                //Disable button
                //ChangeButtonStatus();
                return;
            }

            try
            {
                cashDrawer = (CashDrawer)posExplorer.CreateInstance(deviceInfo);
            }
            catch (Exception)
            {
                //Failed CreateInstance
                //MessageBox.Show("Failed to create instance", MessageBoxButton.OK, MessageBoxImage.Information);
                //MessageBox.Show("Payment confirmed. Thank you!", "Payment Success", MessageBoxButton.OK, MessageBoxImage.Information);

                //Disable button
                //ChangeButtonStatus();
                return;
            }

            //Add StatusUpdateEventHandler
            //AddStatusUpdateEvent(m_Drawer);

            try
            {
                //Open the device
                //Use a Logical Device Name which has been set on the SetupPOS.
                cashDrawer.Open();
            }
            catch (PosControlException)
            {

                //MessageBox.Show("This device has not been registered, or cannot use.", MessageBoxButtons.OK, MessageBoxImage.Information);
                //ChangeButtonStatus();
                return;
            }

            try
            {
                //Get the exclusive control right for the opened device.
                //Then the device is disable from other application.
                cashDrawer.Claim(1000);
            }
            catch (PosControlException)
            {
                //MessageBox.Show("Failed to get exclusive rights to the device.", MessageBoxButtons.OK, MessageBoxImage.Information);
                //ChangeButtonStatus();
                return;
            }

            // Power reporting
            try
            {
                if (cashDrawer.CapPowerReporting != PowerReporting.None)
                {
                    cashDrawer.PowerNotify = PowerNotification.Enabled;
                }
            }
            catch (PosControlException)
            {
            }

            try
            {
                //Enable the device.
                cashDrawer.DeviceEnabled = true;
            }
            catch (PosControlException)
            {

                //MessageBox.Show("Now the device is disable to use.", MessageBoxButtons.OK, MessageBoxImage.Information);

                //ChangeButtonStatus();
                return;
            }
            //<<<step3>>>--End

            //<<<step1>>>--End

            //<<<step4>>>--Start
            //if (m_Drawer.CapStatisticsReporting == false)
            //{
            //    btnRetrieveStatistics.Enabled = false;
            //    txtStatistics.Enabled = false;
            //}
            //<<<step4>>>--End

        }

        private void InitializePrinter()
        {
            //<<<step1>>>--Start
            //Use a Logical Device Name which has been set on the SetupPOS.
            string strLogicalName = "PosPrinter";

            //Current Directory Path
            string strCurDir = Directory.GetCurrentDirectory();

            string strFilePath = strCurDir.Substring(0, strCurDir.LastIndexOf("Step6") + "Step6\\".Length);

            strFilePath += "Logo.bmp";

            try
            {
                //Create PosExplorer
                PosExplorer posExplorer = new PosExplorer();

                DeviceInfo deviceInfo = null;

                try
                {
                    deviceInfo = posExplorer.GetDevice(DeviceType.PosPrinter, strLogicalName);
                    m_Printer = (PosPrinter)posExplorer.CreateInstance(deviceInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed Initialize printer." + ex.Message, "Printer", MessageBoxButton.OK, MessageBoxImage.Warning);
                    //ChangeButtonStatus();
                    return;
                }

                //Open the device
                m_Printer.Open();

                //Get the exclusive control right for the opened device.
                //Then the device is disable from other application.
                m_Printer.Claim(1000);

                //Enable the device.
                m_Printer.DeviceEnabled = true;

                //<<<step3>>>--Start
                //Output by the high quality mode
                m_Printer.RecLetterQuality = true;

                if (m_Printer.CapRecBitmap == true)
                {

                    bool bSetBitmapSuccess = false;
                    for (int iRetryCount = 0; iRetryCount < 5; iRetryCount++)
                    {
                        try
                        {
                            //<<<step5>>>--Start
                            //Register a bitmap
                            m_Printer.SetBitmap(1, PrinterStation.Receipt,
                                strFilePath, m_Printer.RecLineWidth / 2,
                                PosPrinter.PrinterBitmapCenter);
                            //<<<step5>>>--End
                            bSetBitmapSuccess = true;
                            break;
                        }
                        catch (PosControlException pce)
                        {
                            if (pce.ErrorCode == ErrorCode.Failure && pce.ErrorCodeExtended == 0 && pce.Message == "It is not initialized.")
                            {
                                System.Threading.Thread.Sleep(1000);
                            }
                        }
                    }
                    if (!bSetBitmapSuccess)
                    {
                        //MessageBox.Show("Failed to set bitmap.", "Printer_SampleStep6", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //MessageBox.Show("Failed to set bitmap.", "Invoice", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                //<<<step3>>>--End

                //<<<step5>>>--Start
                // Even if using any printers, 0.01mm unit makes it possible to print neatly.
                m_Printer.MapMode = MapMode.Metric;
                //<<<step5>>>--End
            }
            catch (PosControlException ex)
            {


                if (m_Printer != null)
                {
                    try
                    {
                        //Cancel the device
                        m_Printer.DeviceEnabled = false;

                        //Release the device exclusive control right.
                        m_Printer.Release();

                    }
                    catch (PosControlException)
                    {
                    }
                    finally
                    {
                        //Finish using the device.
                        m_Printer.Close();
                    }
                }

                MessageBox.Show("Error in Initialize printer." + ex.Message, "Printer", MessageBoxButton.OK, MessageBoxImage.Warning);
                //ChangeButtonStatus();
            }
            //<<<step1>>>--End
        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Put your code here
            //cashDrawer.Release();
            //m_Printer.Release();
            MessageBox.Show("Application is closing!");

            if (cashDrawer != null)
            {
                try
                {

                    //Cancel the device
                    cashDrawer.DeviceEnabled = false;

                    //Release the device exclusive control right.
                    cashDrawer.Release();

                }
                catch (PosControlException)
                {
                }
                finally
                {
                    //Finish using the device.
                    cashDrawer.Close();
                }
            }

            if (m_Printer != null)
            {
                try
                {
                    //Cancel the device
                    m_Printer.DeviceEnabled = false;

                    //Release the device exclusive control right.
                    m_Printer.Release();

                }
                catch (PosControlException)
                {
                }
                finally
                {
                    //Finish using the device.
                    m_Printer.Close();
                }
            }

            // For example, you could save data, log, etc.
            // SaveData();
        }
    }
}
