   M S B A M L   `   `   `  ����G  CWinePOSFinal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:WinePOSFinal.UserControlsWinePOSFinal.UserControls  5      ,    &WinePOSFinal.UserControls.SalesHistory   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      9local'clr-namespace:WinePOSFinal.UserControls     5      .��5	      �   � RowDefinitions  5
   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      .+�5   
   �� $��0��6    �� Orientation$ Vertical=�6"   $��5q�69   .+�5      �� ���#FFFCF2
�������$��1q�6=   $��0,0,0,10q�6Q   ._�5      �� $ 
Horizontal=�6   $��5q�66   $��Center=�6A   .+�5      �� $��Sales History��6    $��16(�65   $
��Bold"�6C   
��� ���$��Center=�6m   �� 5      -   5        $��PrintInvoiceButton��5      $��Print Invoice��5      $	��120��5      $��30��5      $��Right=�5      $��Center=�5      $��10,0,0,0q�5      5      �� 5       -   5        $��VoidInvoice��5       $��Void Invoice��5!      $	��120��5"      $��30��5#      $	��5,0q�5$      $��Center=�5%      5&      �� 5'      -   5        $��EditInvoice��5'      $��Edit Invoice��5(      $	��120��5)      $��30��5*      $	��5,0q�5+      $��Center=�5,      5-      �� 5.      -   5        $��FlashReportButton��5.      $��Flash Report��5/      $	��120��50      $��30��51      $	��5,0q�52      $��Center=�53      54      �� 55      -   5        $��HourlyReportButton��55      $��Hourly Report��56      $	��120��57      $��30��58      $	��5,0q�59      $��Center=�5:      5;      5<      5=      5>      �� 5A   
   $��1��6   $ 
Horizontal=�6"   $��5q�6;   .+�5B      �� $��From:��6   $��Center=�6%   $	��5,0q�6@   (  "System.Windows.Controls.DatePicker  5C      -   5        $��FromDatePicker��5C      $	��150��6/   $	��5,0q�6;   �� 5E      $	��To:��6   $��Center=�6#   $	��5,0q�6>     5F      -   5        $��ToDatePicker��5F      $	��150��6-   $	��5,0q�69   �� 5H      $��Invoice Number:��6   $��Center=�6/   $	��5,0q�6J   �� 5I      -   5        $��InvoiceNumberTextBox��5I      $	��150��62   $	��5,0q�6>   �� 5K      -	   5        $��SearchButton��5K      $��Search��5L      $	��100��5M      $��30��5N      $	��5,0q�5O      $��Center=�5P      5Q      �� 5S      -
   5        $��ClearButton��5S      $��Clear��5T      $	��100��5U      $��30��5V      $	��5,0q�5W      $��Center=�5X      5Y      �� 5[      -   5        $��TotalPriceLabel��5[      $��Total Price: $0.00��6+   $��Center=�6H   $��16(�6f   $
��Bold"�6t   5\      �� 5_   
   $��2��6   $ Vertical=�6"   .+�5`      &   System.Windows.Controls.DataGrid  -   5        $��SalesInventoryDataGrid��5`         AutoGenerateColumns$ False��65      CanUserAddRows$ False��5a         
IsReadOnly$
 True��6&      SelectionMode$ Extended=�5b         SelectionUnit$ FullRow=�6(   
�������"   VerticalScrollBarVisibility$
 Auto=�5c   &   $	   HorizontalScrollBarVisibility$
	 Auto=�5d      $	��500��64   $
��1200��6A   
   Columns
 5f      0  *System.Windows.Controls.DataGridTextColumn  5g      ,  &System.Windows.Controls.DataGridColumn   Header$ InvoiceCode��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6>   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6H   �� *InvoiceCode+  5h      $	 UPC��6)   $ * 66    6@   �� *UPC+  5i      $ Description��6)   $ * 6>    6H   �� *Name+  5j      $ Price��6)   $ * 68    6B   �� *Price+  5k      $ Quantity��6)   $ * 6;    6E   �� *
Quantity+  5l      $	 Tax��6)   $ * 66    6@   �� *Tax+  5m      $ Total Price��6)   $ * 6>    6H   �� *
TotalPrice+  5n      $ Username��6)   $ * 6;    6E   �� *
UserName+  5o      $ 	Date Time��6)   $ * 6<    6F   �� *CreatedDateTime+  5p      $ Payment Type��6)   $ * 6?    6I   �� *PaymentType+  5q      $ Voided��6)   $ * 69      
Visibility$ 	Collapsed=�6`    6C   �� *
IsVoided+6`   5s      5t      5u      5v      