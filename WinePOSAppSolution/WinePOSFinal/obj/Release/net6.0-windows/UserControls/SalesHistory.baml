   M S B A M L   `   `   `  ���� G  CWinePOSFinal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:WinePOSFinal.UserControlsWinePOSFinal.UserControls  ,    &WinePOSFinal.UserControls.SalesHistory   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     9local'clr-namespace:WinePOSFinal.UserControls     .���   � RowDefinitions  �� $
��Auto ��� $��* � � ColumnDefinitions �� .+��� $��0�� �� Orientation$ Vertical=�$��5q�.+��� ���#FFFCF2
�������$��1q�$��0,0,0,10q�._��� $ 
Horizontal=�$��5q�$��Center=�.+��� $��Sales History��$��16(�$
��Bold"�
��� ���$��Center=��� -   $��PrintInvoiceButton��$��Print Invoice��$	��120��$��30��$��Right=�$��Center=�$��10,0,0,0q��� $��1��$ Vertical=�.+�&   System.Windows.Controls.DataGrid  -   $��SalesInventoryDataGrid��   AutoGenerateColumns$ False��   CanUserAddRows$ False��   
IsReadOnly$
 True��   SelectionMode$ Single=�   SelectionUnit$ FullRow=�
�������"   VerticalScrollBarVisibility$
 Auto=�$	   HorizontalScrollBarVisibility$
	 Auto=�$	��500��$
��1200��
   Columns
 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ InvoiceCode��   Width5  /System.Windows.Controls.DataGridLengthConverter$ * 1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *InvoiceCode+  $	 UPC��$ *  �� *UPC+  $ Description��$ *  �� *Name+  $ Price��$ *  �� *Price+  $ Quantity��$ *  �� *
Quantity+  $	 Tax��$ *  �� *Tax+  $ Total Price��$ *  �� *
TotalPrice+  $ Username��$ *  �� *
UserName+  $ 	Date Time��$ *  �� *CreatedDateTime+