   M S B A M L   `   `   `  ���� E  AOnestopPOS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WinePOSFinalWinePOSFinal  '    !WinePOSFinal.InventoryMaintenance   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     ,localclr-namespace:WinePOSFinal     .���   � RowDefinitions  �� $��1* ��� $
��4.5* ��� $��3* ��� $
��1.5* ��� $
��Auto � � ColumnDefinitions �� .+��� $��0��$��0��.+��� ���#FFFCF2
�������$��1q�$��0,0,0,10q�._���  �� Orientation$ Vertical=�$��5q�.+��� $��Item: ��$��16(�$
��Bold"�
��� ����� $��1��$��0��.+��  �� $��4* ��� $��4* ��� $��2* �  �� $
��Auto �.+��� $��0��$��0��.+��  �� $��4* ��� $��6* �  �� �� �� �� �� �� �� .+��� $ 
Horizontal=�$��Right=�$��0��$��0��$	��0,5q�.+��� $��
Category: ��$	��0,5q�$
��Bold"��� $ 
Horizontal=�$
��Left=�$��0��$��1��$	��0,5q�.+��� -   $��
cbCategory��$	��150��$��25���� $ 
Horizontal=�$��Right=�$��1��$��0��$	��0,5q�.+��� $��UPC: ��$	��0,5q�$
��Bold"��� $ 
Horizontal=�$��1��$
��Left=�$��1��$	��0,5q�.+��� -   $��txtUPC��$	��150��$��25��$��
4900005010���� $ 
Horizontal=�$��Right=�$��2��$��0��$	��0,5q�.+��� $��Description: ��$	��0,5q�$
��Bold"��� $ 
Horizontal=�$��2��$��1��$
��Left=�$	��0,5q�.+��� -   $��txtDescription��$	��150��$��25��$��
COKE 2 L T���� $ 
Horizontal=�$��Right=�$��3��$��0��$	��0,5q�.+��� $��Additional Description: ��$	��0,5q�$
��Bold"��� $ 
Horizontal=�$��3��$��1��$
��Left=�$	��0,5q�.+��� -   $��txtADescription��$	��150��$��25���� $ 
Horizontal=�$��Right=�$��4��$��0��$	��0,5q�.+��� $��Vendor Name: ��$	��0,5q�$
��Bold"��� $ 
Horizontal=�$��4��$��1��$
��Left=�$	��0,5q�.+��� -   $��txtVendorName��$	��150��$��25���� $ 
Horizontal=�$��Right=�$��5��$��0��$	��0,5q�.+��� $��Dropped Item: ��$	��0,5q�$
��Bold"��� $ 
Horizontal=�$��5��$��1��$
��Left=�$	��0,5q�.+��� -   $��txtDroppedItem��$	��150��$��25��${�False��
����ح�
���   ��� $ 
Horizontal=�$��Right=�$��6��$��0��$	��0,5q�.+��� $��Enable Stock Alert: ��$	��0,5q�$
��Bold"�$y�	Collapsed=��� $ 
Horizontal=�$��6��$��1��$
��Left=�$	��0,5q�.+��� -   $��chkStockAlert��$��Enable Stock Alert���� $��0��$��1��.+��  �� $��4* ��� $��6* �  �� �� �� �� �� �� �� .+��� $ 
Horizontal=�$��Right=�$��0��$��0��$	��0,5q�.+��� $��Item Cost($): ��$	��0,5q�$
��Bold"��� $ 
Horizontal=�$
��Left=�$��0��$��1��$	��0,5q�.+��� -   $��txtItemCost��$	��150��$��25��$��
4900005010���� $ 
Horizontal=�$��Right=�$��1��$��0��$	��0,5q�.+��� $��Price You Charge($): ��$	��0,5q�$
��Bold"��� $ 
Horizontal=�$��1��$
��Left=�$��1��$	��0,5q�.+��� -	   $��txtChargePrice��$	��150��$��25���� $ 
Horizontal=�$��Right=�$��2��$��0��$	��0,5q�.+��� $��Price With Sales Tax($): ��$	��0,5q�$
��Bold"�${�False���� $ 
Horizontal=�$��2��$��1��$
��Left=�$	��0,5q�.+��� -
   $��txtPriceWithTax��$	��150��$��25��$��
COKE 2 L T��${�False���� $ 
Horizontal=�$��Right=�$��3��$��0��$	��0,5q�.+��� $��# In Stock: ��$	��0,5q�$
��Bold"��� $ 
Horizontal=�$��3��$��1��$
��Left=�$	��0,5q�.+��� -   $��txtStock��$	��150��$��25���� $ 
Horizontal=�$��Right=�$��4��$��0��$	��0,5q�.+��� $��Case Cost: ��$	��0,5q�$
��Bold"��� $ 
Horizontal=�$��4��$��1��$
��Left=�$	��0,5q�.+��� -   $��txtCaseCost��$	��150��$��25���� $ 
Horizontal=�$��Right=�$��5��$��0��$	��0,5q�.+��� $��# In Case: ��$	��0,5q�$
��Bold"��� $ 
Horizontal=�$��5��$��1��$
��Left=�$	��0,5q�.+��� -   $��txtCase��$	��150��$��25���� $ 
Horizontal=�$��Right=�$��6��$��0��$	��0,5q�.+��� $��Stock Alert Limit: ��$	��0,5q�$
��Bold"��� $ 
Horizontal=�$��6��$��1��$
��Left=�$	��0,5q�.+��� -   $��txtStockAlert��$	��150��$��25���� $��0��$��2��.+��� -   $��txtchkST��$��	Sales Tax���� -   $��txtQuickAdd��$��QuickAdd��$y�	Collapsed=��� $��2��$��0��$��10q�.+��� $��Bulk Pricing��$��16(�$
��Bold"�
��� ���$��0,0,0,10q��  �� $��2* ��� $
��Auto �.+�&   System.Windows.Controls.DataGrid  -   $��dgBulkPricing��$��0��   AutoGenerateColumns$ False��   HeadersVisibility$ Column=�   
IsReadOnly$ False��   CanUserAddRows$
 True��   CanUserDeleteRows$
 True��$	��100��$
��1000��$��0,0,10,0q�"   VerticalScrollBarVisibility$
 Auto=�$	   HorizontalScrollBarVisibility$
	 Auto=�
   Columns
 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ Quantity��   Width5  /System.Windows.Controls.DataGridLengthConverter$	 200 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *
Quantity+ �� UpdateSourceTrigger$ PropertyChanged=�   EditingElementStyle ��  �� 
TargetType" ��.�-     $ Price��$	 200  �� *Price+ �� StringFormat$ {0:F2}��$ PropertyChanged=� �� " ��.�-      RowStyle �� )  #System.Windows.Controls.DataGridRow"  .���  �� Property � �� �� Value$ 30���� $��1��$ Vertical=�$��Right=�$	��Top=�.+��� -   $��
btnAddBulk��$	��Add��
������$��80��$��30��$	��0,5q��� -   $��btnRemoveBulk��$��Remove��
�������$��80��$��30��$	��0,5q��� $��3��$��0��.+�� $��10q�  �� $
��Auto ��� $
��Auto �.+�� $��1��$��Right=�$	��Top=�$��5q�.+��� -   $��btnSave��$��Save Button��
������$	��120��$��35��$��5q��� -   $��btnClear��$��Clear��
�������$	��100��$��35��$��5q��� -   $��btnCopyItem��$��	Copy Item��
������$	��120��$��35��$��5q��� $��4��$ 
Horizontal=�$��Center=�$��10q�.+��� $��Profit: ��$
��Bold"�$��14(�
��� d ��� -    ��Name$ 	lblProfit��$��0%��$
��Bold"�$��14(�
��� d �$
��10,0q��� $��Gross Margin: ��$
��Bold"�$��14(�
����  ��� -   $ lblGrossMargin��$��0%��$
��Bold"�$��14(�
����  �$
��10,0q�