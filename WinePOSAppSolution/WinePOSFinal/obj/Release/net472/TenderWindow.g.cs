﻿#pragma checksum "..\..\..\TenderWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "E630734A2BCB31225BB0F1021202F5B2158F0B54"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using WinePOSFinal;


namespace WinePOSFinal {
    
    
    /// <summary>
    /// TenderWindow
    /// </summary>
    public partial class TenderWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\TenderWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox AmountTextBox;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\TenderWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock RemainingAmountText;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\TenderWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid PaymentGrid;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\TenderWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCustomAmount;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.1.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/OnestopPOS;component/tenderwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\TenderWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.1.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.AmountTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 12 "..\..\..\TenderWindow.xaml"
            this.AmountTextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.AmountTextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 16 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.NumberButton_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 17 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.NumberButton_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 18 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.NumberButton_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 19 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.NumberButton_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 20 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.NumberButton_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 21 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.NumberButton_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 22 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.NumberButton_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 23 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.NumberButton_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 24 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.NumberButton_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            
            #line 25 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OperatorButton_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            
            #line 26 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.NumberButton_Click);
            
            #line default
            #line hidden
            return;
            case 13:
            
            #line 27 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OperatorButton_Click);
            
            #line default
            #line hidden
            return;
            case 14:
            
            #line 28 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.RemoveLastButton_Click);
            
            #line default
            #line hidden
            return;
            case 15:
            
            #line 30 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.DecimalButton_Click);
            
            #line default
            #line hidden
            return;
            case 16:
            this.RemainingAmountText = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 17:
            this.PaymentGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 18:
            
            #line 42 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.QuickTenderButton_Click);
            
            #line default
            #line hidden
            return;
            case 19:
            
            #line 43 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.QuickTenderButton_Click);
            
            #line default
            #line hidden
            return;
            case 20:
            
            #line 44 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.QuickTenderButton_Click);
            
            #line default
            #line hidden
            return;
            case 21:
            
            #line 45 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.QuickTenderButton_Click);
            
            #line default
            #line hidden
            return;
            case 22:
            
            #line 46 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.QuickTenderButton_Click);
            
            #line default
            #line hidden
            return;
            case 23:
            
            #line 47 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.QuickTenderButton_Click);
            
            #line default
            #line hidden
            return;
            case 24:
            this.btnCustomAmount = ((System.Windows.Controls.Button)(target));
            
            #line 48 "..\..\..\TenderWindow.xaml"
            this.btnCustomAmount.Click += new System.Windows.RoutedEventHandler(this.QuickTenderButton_Click);
            
            #line default
            #line hidden
            return;
            case 25:
            
            #line 53 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ClearButton_Click);
            
            #line default
            #line hidden
            return;
            case 26:
            
            #line 54 "..\..\..\TenderWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.DoneButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

