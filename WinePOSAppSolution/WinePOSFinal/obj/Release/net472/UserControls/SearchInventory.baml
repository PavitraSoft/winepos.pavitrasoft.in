   M S B A M L   `   `   `  ���� E  AOnestopPOS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WinePOSFinalWinePOSFinal  "    WinePOSFinal.SearchInventory   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     ,localclr-namespace:WinePOSFinal     .���   � RowDefinitions  �� $��1* ��� $��8* � � ColumnDefinitions �� .+��� $��0��$��0��.+��� ���#FFFCF2
�������$��1q�$��0,0,0,10q�._���  �� Orientation$ Vertical=�$��5q�.+��� $��Search Inventory��$��16(�$
��Bold"�
��� ����� $��1��$��0��.+��   �� $��1* � �� $��7* ��� $��3* �.+��� $��0��$��0��.+�&   System.Windows.Controls.DataGrid  -   $��InventoryDataGrid��   AutoGenerateColumns$ False��   CanUserAddRows$ False��   
IsReadOnly$ False��   SelectionMode$ Single=�   SelectionUnit$ FullRow=�
�������"   VerticalScrollBarVisibility$
 Auto=�$	   HorizontalScrollBarVisibility$
	 Auto=�$	��500��$	��800��
   Columns
 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$	 UPC��   Width5  /System.Windows.Controls.DataGridLengthConverter$ *    
IsReadOnly$
 True��1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *UPC+   EditingElementStyle ��  �� 
TargetType" ��.�-     $ Description��$ *  �� *Description+  $ 	Item Cost��$ *  �� *
ItemCost+ �� " ��.�-     $ 
Price Cost��$ *  �� *ChargedCost+ �� " ��.�-     $ 	Sales Tax��$ * $
 True�� �� *
SalesTax+ �� " ��  $ Stock��$ *  �� *Stock+ �� " ��.�-     $ Additional Description��$ *  �� *AdditionalDescription+  $ Vendor Part Num.��$ *  �� *VendorPartNum+ �� " ��.�-     $ Vendor Name.��$ *  �� *
VendorName+�� $��0��$��1��.+��� $
��UPC:��$
��Bold"�$��Center=�$	��Top=��� -   $��txtUPC��$	��200��$��25��$	��Top=��� $��Name:��$
��Bold"�$��Center=�$	��Top=��� -   $��txtDescription��$	��200��$��25��$	��Top=��� -	   $��	btnSearch��$��🔍 Search��$��75��$��25��$	��Top=�$��2q��� -
   $��btnClear��$��Clear��$��75��$��25��$	��Top=�$��2q��� -   $��btnEdit��$��	Edit Item��
�������$	��120��$��40��$	��Top=�$��5q��� -   $��btnAdd��$��Add Item��
��� ���$	��120��$��40��$	��Top=�$��5q��� -   $��	btnDelete��$��Delete��
���  ��$	��120��$��40��$	��Top=�$��5q��� -   $��	btnExport��$��Export��
��� � �$	��120��$��40��$	��Top=�$��5q��� -   $��	btnImport��$��Import��
����  �$	��120��$��40��$	��Top=�$��5q�