   M S B A M L   `   `   `  ���� E  AOnestopPOS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WinePOSFinalWinePOSFinal      WinePOSFinal.Billing   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     ,localclr-namespace:WinePOSFinal     .���   � RowDefinitions  �� $	��100 ��� $	��100 ��� $��* ��� $��50 �.+�� $��0�� � ColumnDefinitions �� $��2* ��� $��1* ��� $��1* ��� $
��Auto ��� $
��Auto �  �� $��2* ��� $��8* �.+��� $
��UPC:��$��0��$��0��$
��Bold"�$
��Left=�$	��Top=��� -   $��txtUPC��$��0��$��1��$��40��$	��Top=�$��5q�$��16(��� $��Name:��$��1��$��0��$
��Bold"�$
��Left=�$	��Top=�� $��1��$��1��.+��� -   $��txtName��$��40��$	��Top=�$��5q�$��16(�� -   $��NameSuggestionsPopup��$��Bottom=� � 	StaysOpen$ False�� � PlacementTarget ��  �� ElementName$ txtName��.���� -   $��lstNameSuggestions������ $ txtName�� �� Path$ ActualWidth��� $��	Quantity:��$��2��$��0��$
��Bold"�$
��Left=�$	��Top=��� -   $��txtQuantity��$��2��$��1��$��40��$	��Top=�$��5q�$��16(��� -   $��	btnSearch��$��3��$��1��$	��100��$��40��$��	5,-25,5,5q�$��Search���� -   $��btnAdd��$��4��$��1��$	��100��$��40��$��	5,-25,5,5q�$	��Add���� $��1�� �� Orientation$ Vertical=�$��5q�.+��� ���#FFFCF2
�������$��1q�$��0,0,0,10q�._��� $ 
Horizontal=�$��5q�$��Center=�.+��� $��	Quick Add��$��16(�$
��Bold"�
���   �$��Center=��� -   $��NonScanNoTax��$��Non Scan No Tax��$	��120��$��30��$��Right=�$��Center=�$��10,0,0,0q��� -	   $��IceBag��$��ICE BAG��$	��120��$��30��$	��5,0q�$��Center=��� -
   $
��Nuts��$
��NUTS��$	��120��$��30��$	��5,0q�$��Center=��� -   $��textDiscount��$��	Discount:��$��3��$
��Bold"�$��Right=�$��Center=�$��5q��� -   $��txtDiscountValue��$��4��$	��200��$��30��$
��Left=�$��Center=�$��0q��� -   $��btnApplyDiscount��$��5��$	��100��$��30��$��0q�$��Apply��$
��Left=�� $��2��$��5q� �� $��3* ��� $��1* �.+�&   System.Windows.Controls.DataGrid  -   $��	dgBilling��$��0��   AutoGenerateColumns$ False��   HeadersVisibility$ Column=�	   CanUserAddRows$	 False��
   SelectionMode$
 Single=�   SelectionUnit$ FullRow=�"   VerticalScrollBarVisibility$
 Auto=�$   HorizontalScrollBarVisibility$
 Auto=� �� SelectedItem �� *SelectedBillingItem+   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$	 UPC��   Width5  /System.Windows.Controls.DataGridLengthConverter$ *    
IsReadOnly$
 True��1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *UPC+  $
 Name��$ * $
 True�� �� *Name+  $ Price��$ * $ False�� �� *Price+  $ Quantity��$ * $
 True�� �� *
Quantity+  $	 Tax��$ * $
 True�� �� *Tax+  $ Total Price��$ * $
 True�� �� *
TotalPrice+  $ Discount (%)��$ *  �� *
Discount+ �� Mode$ TwoWay=�  $
 Note��$ *  �� *Note+$ TwoWay=�� $��1��$��10q�  �� $��7* ��� $��3* �.+��� $��0��
�������.+��� $��	Sub Total��$��16(��� -   $��txtTotal��$��20(�$
��Bold"����� *
SubTotal+ �� StringFormat$ C���� $	��Tax��$��16(��� -   $��txtTax��$��20(�$
��Bold"����� *Tax+$ C���� $��Discount��$��16(�$y�	Collapsed=��� -   $��txtDiscount��$��20(�$
��Bold"�$y�	Collapsed=����� *
Discount+$ C���� $��Grand Total��$��16(��� -   $��txtGrandTotal��$��20(�$
��Bold"����� *
GrandTotal+$ C���� -   $��	Remaining��$��Remaining Amount��$��16(�$y�	Collapsed=��� -   $��txtAmtRemaining��$��20(�$
��Bold"�$y�	Collapsed=��� -   $��Change��$��Change��$��16(�$y�	Collapsed=��� -   $��txtAmtChange��$��20(�$
��Bold"�$y�	Collapsed=�A� $��1�� A� Rows$ 4�� A� Columns$ 2��$��Stretch=�$��Stretch=�$��5q�.+��� -   $��
btnPalmPay��$��PAY TO PALM PAY��
��� � �
��������� -   $��btnCash��$
��CASH��
��� � ��� -   $��btnCheck��$��CHECK��
��� ����� -   $��	btnCredit��$��CREDIT/DEBIT��
��� ����� -   $��btnSplitPayment��$��Split Payment��
��� � ��� -   $��btnVoidInvoice��$��CLEAR INVOICE��
���  ��
��������� -   $��btnRemoveItem��$��REMOVE ITEM��
���  ���� -   $��btnTenderWindow��$��TENDER WINDOW��
��� ����� -   $��btnTaxConfig��$��
TAX CONFIG��
���  ���� -    $��btnCashDrawer��$��CASH DRAWER��
���  ��