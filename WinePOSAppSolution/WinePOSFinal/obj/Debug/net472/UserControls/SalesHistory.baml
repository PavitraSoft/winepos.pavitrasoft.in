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
��� ���$��Center=�6m   �� 5      -   5        $��PrintInvoiceButton��5      $��Print Invoice��5      $	��120��5      $��30��5      $��Right=�5      $��Center=�5      $��10,0,0,0q�5      5      �� 5       -   5        $��FlashReportButton��5       $��Flash Report��5!      $	��120��5"      $��30��5#      $	��5,0q�5$      $��Center=�5%      5&      �� 5'      -   5        $��VoidInvoice��5'      $��Void Invoice��5(      $	��120��5)      $��30��5*      $	��5,0q�5+      $��Center=�5,      5-      5.      5/      50      �� 53   
   $��1��6   $ 
Horizontal=�6"   $��5q�6;   .+�54      �� $��From:��6   $��Center=�6%   $	��5,0q�6@   (  "System.Windows.Controls.DatePicker  55      -   5        $��FromDatePicker��55      $	��150��6/   $	��5,0q�6;   �� 57      $	��To:��6   $��Center=�6#   $	��5,0q�6>     58      -   5        $��ToDatePicker��58      $	��150��6-   $	��5,0q�69   �� 5:      $��Invoice Number:��6   $��Center=�6/   $	��5,0q�6J   �� 5;      -   5        $��InvoiceNumberTextBox��5;      $	��150��62   $	��5,0q�6>   �� 5=      -   5        $��SearchButton��5=      $��Search��5>      $	��100��5?      $��30��5@      $	��5,0q�5A      $��Center=�5B      5C      �� 5E      -   5        $��ClearButton��5E      $��Clear��5F      $	��100��5G      $��30��5H      $	��5,0q�5I      $��Center=�5J      5K      �� 5M      -	   5        $��TotalPriceLabel��5M      $��Total Price: $0.00��6+   $��Center=�6H   $��16(�6f   $
��Bold"�6t   5N      �� 5Q   
   $��2��6   $ Vertical=�6"   .+�5R      &   System.Windows.Controls.DataGrid  -
   5        $��SalesInventoryDataGrid��5R         AutoGenerateColumns$ False��65      CanUserAddRows$ False��5S         
IsReadOnly$
 True��6&      SelectionMode$ Extended=�5T         SelectionUnit$ FullRow=�6(   
�������"   VerticalScrollBarVisibility$
 Auto=�5U   &   $	   HorizontalScrollBarVisibility$
	 Auto=�5V      $	��500��64   $
��1200��6A   
   Columns
 5X      0  *System.Windows.Controls.DataGridTextColumn  5Y      ,  &System.Windows.Controls.DataGridColumn   Header$ InvoiceCode��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6>   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6H   �� *InvoiceCode+  5Z      $	 UPC��6)   $ * 66    6@   �� *UPC+  5[      $ Description��6)   $ * 6>    6H   �� *Name+  5\      $ Price��6)   $ * 68    6B   �� *Price+  5]      $ Quantity��6)   $ * 6;    6E   �� *
Quantity+  5^      $	 Tax��6)   $ * 66    6@   �� *Tax+  5_      $ Total Price��6)   $ * 6>    6H   �� *
TotalPrice+  5`      $ Username��6)   $ * 6;    6E   �� *
UserName+  5a      $ 	Date Time��6)   $ * 6<    6F   �� *CreatedDateTime+  5b      $ Payment Type��6)   $ * 6?    6I   �� *PaymentType+  5c      $ Voided��6)   $ * 69      
Visibility$ 	Collapsed=�6`    6C   �� *
IsVoided+6`   5e      5f      5g      5h      