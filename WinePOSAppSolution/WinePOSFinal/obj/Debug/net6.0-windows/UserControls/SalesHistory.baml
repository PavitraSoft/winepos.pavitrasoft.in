   M S B A M L   `   `   `  ����G  CWinePOSFinal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:WinePOSFinal.UserControlsWinePOSFinal.UserControls  5      ,    &WinePOSFinal.UserControls.SalesHistory   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      9local'clr-namespace:WinePOSFinal.UserControls     5      .��5	      �   � RowDefinitions  5
   
   �� 5      $
��Auto �6   6,   �� 5      $��* �6   6)    � ColumnDefinitions 5   
   �� 5      .+�5   
   �� $��0��6    �� Orientation$ Vertical=�6"   $��5q�69   .+�5      �� ���#FFFCF2
�������$��1q�6=   $��0,0,0,10q�6Q   ._�5      �� $ 
Horizontal=�6   $��5q�66   $��Center=�6A   .+�5      �� $��Sales History��6    $��16(�65   $
��Bold"�6C   
��� ���$��Center=�6m   �� 5      -   5        $��PrintInvoiceButton��5      $��Print Invoice��5      $	��120��5      $��30��5      $��Right=�5      $��Center=�5      $��10,0,0,0q�5      5      5      5       5!      �� 5$   
   $��1��6   $ Vertical=�6"   .+�5%      &   System.Windows.Controls.DataGrid  -   5        $��SalesInventoryDataGrid��5%         AutoGenerateColumns$ False��65      CanUserAddRows$ False��5&         
IsReadOnly$
 True��6.      SelectionMode$ Single=�5'         SelectionUnit$ FullRow=�6.   
�������"   VerticalScrollBarVisibility$
 Auto=�5(   .   $	   HorizontalScrollBarVisibility$
	 Auto=�5)      $	��500��6<   $
��1200��6I   
   Columns
 5+      0  *System.Windows.Controls.DataGridTextColumn  5,      ,  &System.Windows.Controls.DataGridColumn   Header$ InvoiceCode��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6>   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6H   �� *InvoiceCode+  5-      $	 UPC��6)   $ * 66    6@   �� *UPC+  5.      $ Description��6)   $ * 6>    6H   �� *Name+  5/      $ Price��6)   $ * 68    6B   �� *Price+  50      $ Quantity��6)   $ * 6;    6E   �� *
Quantity+  51      $	 Tax��6)   $ * 66    6@   �� *Tax+  52      $ Total Price��6)   $ * 6>    6H   �� *
TotalPrice+  53      $ Username��6)   $ * 6;    6E   �� *
UserName+  54      $ 	Date Time��6)   $ * 6<    6F   �� *CreatedDateTime+56      57      58      59      