   M S B A M L   `   `   `  ���� G  CWinePOSFinal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WinePOSFinalWinePOSFinal      WinePOSFinal.Billing   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     ,localclr-namespace:WinePOSFinal     .���   � RowDefinitions  �� $	��100 ��� $	��100 ��� $��* ��� $��50 �.+�� $��0�� � ColumnDefinitions �� $��2* ��� $��1* ��� $��1* ��� $
��Auto ��� $
��Auto �  �� $��2* ��� $��8* �.+��� $
��UPC:��$��0��$��0��$
��Bold"�$
��Left=�$	��Top=��� -   $��txtUPC��$��0��$��1��$��40��$	��Top=�$��5q�$��16(��� $��Name:��$��1��$��0��$
��Bold"�$
��Left=�$	��Top=��� -   $��txtName��$��1��$��1��$��40��$	��Top=�$��5q�$��16(�${�False���� $��	Quantity:��$��2��$��0��$
��Bold"�$
��Left=�$	��Top=��� -   $��txtQuantity��$��2��$��1��$��40��$	��Top=�$��5q�$��16(��� -   $��	btnSearch��$��3��$��1��$	��100��$��40��$��	5,-25,5,5q�$��Search���� -   $��btnAdd��$��4��$��1��$	��100��$��40��$��	5,-25,5,5q�$	��Add��� $��1�� �� $
��Auto ��� $��2* ��� $��8* �.+��� $��
Quick ADD:��$��0��$
��Bold"�$
��Left=�$��Center=�$��5q��� -   $��
cbQuickADD��$��1��$	��150��$��30��$
��Left=�$��5q��� -   $��btnQuickAdd��$��2��$	��100��$��40��$��5q�$	��Add��$
��Left=�� $��2��$��5q� �� $��3* ��� $��1* �.+�&   System.Windows.Controls.DataGrid  -   $��	dgBilling��$��0��   AutoGenerateColumns$ False��   HeadersVisibility$ Column=�   CanUserAddRows$ False��   SelectionMode$ Single=�   SelectionUnit$ FullRow=�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$	 UPC��	   Width5  /System.Windows.Controls.DataGridLengthConverter$	 * 1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 �� *UPC+  $
 Name��$	 * 
 �� *Name+  $ Price��$	 * 
 �� *Price+  $ Quantity��$	 * 
 �� *
Quantity+  $	 Tax��$	 * 
 �� *Tax+  $ Total Price��$	 * 
 �� *
TotalPrice+� $��1��$��10q�  �� $��8* ��� $��2* �.+��� $��0��
�������.+��� $��	Sub Total��$��16(��� -	   $��txtTotal��$��20(�$
��Bold"����� *
SubTotal+ �� StringFormat$ C���� $	��Tax��$��16(��� -
   $��txtTax��$��20(�$
��Bold"����� *Tax+$ C���� $��Grand Total��$��16(��� -   $��txtGrandTotal��$��20(�$
��Bold"����� *
GrandTotal+$ C��A� $��1�� A� Rows$ 4�� A� Columns$ 2��$��Stretch=�$��Stretch=�$��5q�.+��� -   $��
btnPalmPay��$��PAY TO PALM PAY��
��� � �
��������� -   $��btnCash��$
��CASH���� -   $��btnVoidInvoice��$��VOID INVOICE��
���  ��
��������� -   $��btnPrintInvoice��$��PRINT INVOICE��
��� ����� -   $��btnRemoveItem��$��REMOVE ITEM��
���  ���� -   $��btnCheck��$��CHECK���� -   $��	btnCredit��$��CREDIT/DEBIT��