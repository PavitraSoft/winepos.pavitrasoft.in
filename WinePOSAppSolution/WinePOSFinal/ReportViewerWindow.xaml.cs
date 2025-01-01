using CrystalDecisions.Shared;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using WinePOSFinal.Classes;
using WinePOSFinal.ServicesLayer;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Configuration;
using System.Linq;
using System.Data.SqlClient;
using System.IO;

namespace WinePOSFinal
{
    /// <summary>
    /// Interaction logic for ReportViewerWindow.xaml
    /// </summary>
    public partial class ReportViewerWindow : Window
    {
        public ReportViewerWindow()
        {
            InitializeComponent();
        }
        public void SetReport(ReportDocument reportDocument)
        {
            CrystalReportViewer.ReportSource = reportDocument;
            CrystalReportViewer.Refresh();
        }
    }
}
