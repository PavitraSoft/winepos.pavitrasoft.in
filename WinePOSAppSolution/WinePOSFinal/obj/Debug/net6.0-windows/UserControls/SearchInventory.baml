   M S B A M L   `   `   `  ����G  CWinePOSFinal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WinePOSFinalWinePOSFinal  5      "    WinePOSFinal.SearchInventory   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ,localclr-namespace:WinePOSFinal     5      .��5	      �   � RowDefinitions  5
   
   �� 5      $��1* �6   6*   �� 5      $��8* �6   6*    � ColumnDefinitions 5   
   �� 5      .+�5   
   �� $��0��6   $��0��6"   .+�5      �� ���#FFFCF2
�������$��1q�6=   $��0,0,0,10q�6Q   ._�5      ��  �� Orientation$ Vertical=�6   $��5q�64   .+�5      �� $��Search Inventory��6    $��16(�68   $
��Bold"�6F   
��� ���6X   5      5      5      �� 5   
   $��1��6   $��0��6"   .+�5      �   5      �� 5      $��1* �6$   62    5      �� 5       $��7* �6'   �� 5!      $��3* �6'   .+�5$      �� $��0��6   $��0��6*   .+�5&      &   System.Windows.Controls.DataGrid  -   5        $��InventoryDataGrid��5&         AutoGenerateColumns$ False��68      CanUserAddRows$ False��6T      
IsReadOnly$ False��6k      SelectionMode$ Single=�6~      SelectionUnit$ FullRow=�6�   
�������"   VerticalScrollBarVisibility$
 Auto=�6-  $	   HorizontalScrollBarVisibility$
	 Auto=�6P  $	��500��6u  $	��800��6�  
   Columns
 5'      0  *System.Windows.Controls.DataGridTextColumn  5(      ,  &System.Windows.Controls.DataGridColumn   Header$	 UPC��61      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6>   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6H   �� *UPC+   EditingElementStyle 5)   "   �� 5*   &    �� 
TargetType" ��6,   .�5+   *   -   5        5,   '   5.        5/      $ Description��61   $ * 6F    6P   �� *Description+  50      $ 	Item Cost��61   $ * 6D    6N   �� *
ItemCost+ 51   "   �� 52   &   " ��6,   .�53   *   -   5        54   '   56        57      $ 
Price Cost��61   $ * 6E    6O   �� *ChargedCost+ 58   "   �� 59   &   " ��6,   .�5:   *   -   5        5;   '   5=        5>      $ 	Sales Tax��61   $ * 6D    6N   �� *
SalesTax+ 5?   "   �� 5@   &   " ��6,   .�5A   *   -   5        5B   '   5D        5E      $ Stock��61   $ * 6@    6J   �� *Stock+ 5F   "   �� 5G   &   " ��6,   .�5H   *   -   5        5I   '   5K        5L      $ Additional Description��61   $ * 6Q    6[   �� *AdditionalDescription+  5M      $ Vendor Part Num.��61   $ * 6K    6U   �� *VendorPartNum+ 5N   "   �� 5O   &   " ��6,   .�5P   *   -   5        5Q   '   5S        5T      $ Vendor Name.��61   $ * 6G    6Q   �� *
VendorName+5V      5W      �� 5X      $��0��6   $��1��6*   .+�5Z      �� $
��UPC:��6    $
��Bold"�6,   $��Center=�6>   $	��Top=�6[   �� 5[      -   5        $��txtUPC��5[      $	��200��6,   $��25��68   $	��Top=�6D   �� 5\      $��Name:��6    $
��Bold"�6-   $��Center=�6?   $	��Top=�6\   �� 5]      -	   5        $��txtDescription��5]      $	��200��64   $��25��6@   $	��Top=�6L   �� 5^      -
   5        $��	btnSearch��5^      $��🔍 Search��6.   $��75��6B   $��25��6M   $	��Top=�6Y   $��2q�6q   6|   �� 5_      -   5        $��btnClear��5_      $��Clear��6-   $��75��6=   $��25��6H   $	��Top=�6T   $��2q�6l   6w   �� 5b      -   5        $��btnEdit��5b      $��	Edit Item��6,   
�������$	��120��6S   $��40��6_   $	��Top=�6k   $��5q�6�   6�   �� 5c      -   5        $��btnAdd��5c      $��Add Item��6+   
��� ���$	��120��6R   $��40��6^   $	��Top=�6j   $��5q�6�   6�   �� 5d      -   5        $��	btnDelete��5d      $��Delete��6.   
���  ��$	��120��6P   $��40��6\   $	��Top=�6h   $��5q�6�   6�   �� 5f      -   5        $��	btnExport��5f      $��Export��6.   
��� � �$	��120��6R   $��40��6^   $	��Top=�6j   $��5q�6�   6�   �� 5h      -   5        $��	btnImport��5h      $��Import��6.   
����  �$	��120��6Q   $��40��6]   $	��Top=�6i   $��5q�6�   6�   5k      5l      5n      5o      5p      