   M S B A M L   `   `   `  ����G  CWinePOSFinal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WinePOSFinalWinePOSFinal  5      '    !WinePOSFinal.InventoryMaintenance   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ,localclr-namespace:WinePOSFinal     5      .��5	      �   � RowDefinitions  5   
   �� 5      $��1* �6   6*   �� 5      $
��4.5* �6   6,   �� 5      $��3* �6   6*   �� 5      $
��1.5* �6   6,   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      .+�5   
   �� $��0��6   $��0��6"   .+�5      �� ���#FFFCF2
�������$��1q�6=   $��0,0,0,10q�6Q   ._�5      ��  �� Orientation$ Vertical=�6   $��5q�64   .+�5      �� $��Item: ��6    $��16(�6.   $
��Bold"�6<   
��� ���6N   5      5      5      �� 5   
   $��1��6   $��0��6"   .+�5      �  5      �� 5       $��4* �6'   �� 5!      $��4* �6'   �� 5"      $��2* �6'     5$      �� 5%      $
��Auto �6$   .+�5)      �� $��0��6   $��0��6*   .+�5*      �  5+      �� 5,      $��4* �6/   �� 5-      $��6* �6/     5/      �� 50      �� 51      �� 52      �� 53      �� 54      �� 55      �� 56      .+�58      �� $ 
Horizontal=�6%   $��Right=�6>   $��0��6Z   $��0��6g   $	��0,5q�6w   .+�59      �� $��
Category: ��6(   $	��0,5q�6:   $
��Bold"�6G   5:      �� 5;      $ 
Horizontal=�6%   $
��Left=�6>   $��0��6Y   $��1��6f   $	��0,5q�6v   .+�5<      �� -   5        $��
cbCategory��5<   '   $	��150��69   $��25��6E   5=      5>      �� 5?      $ 
Horizontal=�6%   $��Right=�6>   $��1��6Z   $��0��6g   $	��0,5q�6w   .+�5@      �� $��UPC: ��6(   $	��0,5q�65   $
��Bold"�6B   5A      �� 5B      $ 
Horizontal=�6%   $��1��6>   $
��Left=�6K   $��1��6f   $	��0,5q�6v   .+�5C      �� -   5        $��txtUPC��5C   &   $	��150��64   $��25��6@   $��
4900005010��6L   5D      �� 5E      $ 
Horizontal=�6%   $��Right=�6>   $��2��6Z   $��0��6g   $	��0,5q�6w   .+�5F      �� $��Description: ��6(   $	��0,5q�6=   $
��Bold"�6J   5G      �� 5H      $ 
Horizontal=�6%   $��2��6>   $��1��6K   $
��Left=�6[   $	��0,5q�6v   .+�5I      �� -   5        $��txtDescription��5I   &   $	��150��6<   $��25��6H   $��
COKE 2 L T��6T   5J      �� 5K      $ 
Horizontal=�6%   $��Right=�6>   $��3��6Z   $��0��6g   $	��0,5q�6w   .+�5L      �� $��Additional Description: ��6(   $	��0,5q�6H   $
��Bold"�6U   5M      �� 5N      $ 
Horizontal=�6%   $��3��6>   $��1��6K   $
��Left=�6[   $	��0,5q�6v   .+�5O      �� -   5        $��txtADescription��5O   &   $	��150��6=   $��25��6I   5P      �� 5Q      $ 
Horizontal=�6%   $��Right=�6>   $��4��6Z   $��0��6g   $	��0,5q�6w   .+�5R      �� $��Vendor Name: ��6(   $	��0,5q�6=   $
��Bold"�6J   5S      �� 5T      $ 
Horizontal=�6%   $��4��6>   $��1��6K   $
��Left=�6[   $	��0,5q�6v   .+�5U      �� -   5        $��txtVendorName��5U   &   $	��150��6;   $��25��6G   5V      �� 5W      $ 
Horizontal=�6%   $��Right=�6>   $��5��6Z   $��0��6g   $	��0,5q�6w   .+�5X      �� $��Dropped Item: ��6(   $	��0,5q�6>   $
��Bold"�6K   5Y      �� 5[      $ 
Horizontal=�6%   $��5��6>   $��1��6K   $
��Left=�6[   $	��0,5q�6v   .+�5\      �� -   5        $��txtDroppedItem��5\   &   $	��150��6<   $��25��6H   ${�False��5]      
����ح�
���   �5_      5`      �� 5a      $ 
Horizontal=�6%   $��Right=�6>   $��6��6Z   $��0��6g   $	��0,5q�6w   .+�5b      �� $��Enable Stock Alert: ��6(   $	��0,5q�6D   $
��Bold"�6Q   $y�	Collapsed=�6c   5c      �� 5d      $ 
Horizontal=�6%   $��6��6>   $��1��6K   $
��Left=�6[   $	��0,5q�6v   .+�5e      �� -   5        $��chkStockAlert��5e   '   $��Enable Stock Alert��6<   6z   5f      5g      5h      �� 5j      $��0��6   $��1��6*   .+�5k      �  5l      �� 5m      $��4* �6/   �� 5n      $��6* �6/     5p      �� 5q      �� 5r      �� 5s      �� 5t      �� 5u      �� 5v      �� 5w      .+�5y      �� $ 
Horizontal=�6%   $��Right=�6>   $��0��6Z   $��0��6g   $	��0,5q�6w   .+�5z      �� $��Item Cost($): ��6(   $	��0,5q�6>   $
��Bold"�6K   5{      �� 5|      $ 
Horizontal=�6%   $
��Left=�6>   $��0��6Y   $��1��6f   $	��0,5q�6v   .+�5}      �� -   5        $��txtItemCost��5}   &   $	��150��69   $��25��6E   $��
4900005010��6Q   5~      �� 5      $ 
Horizontal=�6%   $��Right=�6>   $��1��6Z   $��0��6g   $	��0,5q�6w   .+�5�      �� $��Price You Charge($): ��6(   $	��0,5q�6E   $
��Bold"�6R   5�      �� 5�      $ 
Horizontal=�6%   $��1��6>   $
��Left=�6K   $��1��6f   $	��0,5q�6v   .+�5�      �� -	   5        $��txtChargePrice��5�   &   $	��150��6<   $��25��6H   6T   5�      �� 5�      $ 
Horizontal=�6%   $��Right=�6>   $��2��6Z   $��0��6g   $	��0,5q�6w   .+�5�      �� $��Price With Sales Tax($): ��6(   $	��0,5q�6I   $
��Bold"�6V   ${�False��6h   5�      �� 5�      $ 
Horizontal=�6%   $��2��6>   $��1��6K   $
��Left=�6[   $	��0,5q�6v   .+�5�      �� -
   5        $��txtPriceWithTax��5�   &   $	��150��6=   $��25��6I   $��
COKE 2 L T��6U   ${�False��6g   5�      �� 5�      $ 
Horizontal=�6%   $��Right=�6>   $��3��6Z   $��0��6g   $	��0,5q�6w   .+�5�      �� $��# In Stock: ��6(   $	��0,5q�6<   $
��Bold"�6I   5�      �� 5�      $ 
Horizontal=�6%   $��3��6>   $��1��6K   $
��Left=�6[   $	��0,5q�6v   .+�5�      �� -   5        $��txtStock��5�   &   $	��150��66   $��25��6B   5�      �� 5�      $ 
Horizontal=�6%   $��Right=�6>   $��4��6Z   $��0��6g   $	��0,5q�6w   .+�5�      �� $��Case Cost: ��6(   $	��0,5q�6;   $
��Bold"�6H   5�      �� 5�      $ 
Horizontal=�6%   $��4��6>   $��1��6K   $
��Left=�6[   $	��0,5q�6v   .+�5�      �� -   5        $��txtCaseCost��5�   &   $	��150��69   $��25��6E   6Q   5�      �� 5�      $ 
Horizontal=�6%   $��Right=�6>   $��5��6Z   $��0��6g   $	��0,5q�6w   .+�5�      �� $��# In Case: ��6(   $	��0,5q�6;   $
��Bold"�6H   5�      �� 5�      $ 
Horizontal=�6%   $��5��6>   $��1��6K   $
��Left=�6[   $	��0,5q�6v   .+�5�      �� -   5        $��txtCase��5�   &   $	��150��65   $��25��6A   6M   5�      �� 5�      $ 
Horizontal=�6%   $��Right=�6>   $��6��6Z   $��0��6g   $	��0,5q�6w   .+�5�      �� $��Stock Alert Limit: ��6(   $	��0,5q�6C   $
��Bold"�6P   5�      �� 5�      $ 
Horizontal=�6%   $��6��6>   $��1��6K   $
��Left=�6[   $	��0,5q�6v   .+�5�      �� -   5        $��txtStockAlert��5�   &   $	��150��6;   $��25��6G   5�      5�      5�      �� 5�      $��0��6   $��2��6*   .+�5�      �� -   5        $��txtchkST��5�      $��	Sales Tax��6/   6^   �� 5�      -   5        $��txtQuickAdd��5�      $��QuickAdd��62   $y�	Collapsed=�6E   5�      5�      5�      �� 58  
   $��2��6   $��0��6"   $��10q�62   .+�59     �� $��Bulk Pricing��6   $��16(�6,   $
��Bold"�6:   
��� ���$��0,0,0,10q�6d   � 5<      5=     �� 5>     $��2* �6'   �� 5@     $
��Auto �6'   .+�5E     &   System.Windows.Controls.DataGrid  -   5        $��dgBulkPricing��5E     $��0��60      AutoGenerateColumns$ False��6@      HeadersVisibility$ Column=�6\      
IsReadOnly$ False��5F        CanUserAddRows$
 True��6      CanUserDeleteRows$
 True��64   $	��100��5G     $
��1000��6   $��0,0,10,0q�6%   "   VerticalScrollBarVisibility$
 Auto=�67   $	   HorizontalScrollBarVisibility$
	 Auto=�6Z   
   Columns
 5H     0  *System.Windows.Controls.DataGridTextColumn  5J     ,  &System.Windows.Controls.DataGridColumn   Header$ Quantity��6-      Width5  /System.Windows.Controls.DataGridLengthConverter$	 200 6�   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6?   �� *
Quantity+ �� UpdateSourceTrigger$ PropertyChanged=�   EditingElementStyle 5K     �� 5L  "    �� 
TargetType" ��6(   .�5M  &   -   5        5N  #   5P       5S     $ Price��6-   $	 200 6�    6<   �� *Price+ �� StringFormat$ {0:F2}��$ PropertyChanged=� 5T     �� 5U  "   " ��6(   .�5V  &   -   5        5W  #   5Y        RowStyle 5\     �� 5]     )  #System.Windows.Controls.DataGridRow"  6    .�5^     ��  �� Property � �� �� Value$ 30��67   5_     5a     �� 5d     $��1��6   $ Vertical=�6-   $��Right=�6D   $	��Top=�6`   .+�5e     �� -   5        $��
btnAddBulk��5e     $	��Add��6/   
������$��80��6U   $��30��6`   $	��0,5q�6l   6y   �� 5f     -   5        $��btnRemoveBulk��5f     $��Remove��62   
�������$��80��6[   $��30��6f   $	��0,5q�6r   6   5g     5h     5i     �� 5k  
   $��3��6   $��0��6"   .+�5l     � $��10q�6     5m     �� 5n     $
��Auto �6$   �� 5o     $
��Auto �6$   .+�5{     � $��1��6   $��Right=�6)   $	��Top=�6E   $��5q�6]   .+�5}     �� -   5        $��btnSave��5}     $��Save Button��6,   
������$	��120��6Z   $��35��6f   $��5q�6r   6}   �� 5�     -   5        $��btnClear��5�     $��Clear��6-   
�������$	��100��6U   $��35��6a   $��5q�6m   6x   �� 5�     -   5        $��btnCopyItem��5�     $��	Copy Item��60   
������$	��120��6\   $��35��6h   $��5q�6t   6   5�     5�     5�     �� 5�  
   $��4��6   $ 
Horizontal=�6"   $��Center=�6;   $��10q�6X   .+�5�     �� $��Profit: ��6   $
��Bold"�6'   $��14(�69   
��� d �6G   �� 5�     -   5         ��Name$ 	lblProfit��5�     $��0%��6'   $
��Bold"�64   $��14(�6F   
��� d �$
��10,0q�6k   �� 5�     $��Gross Margin: ��6   $
��Bold"�6-   $��14(�6?   
����  �6M   �� 5�     -   5        $ lblGrossMargin��5�     $��0%��6,   $
��Bold"�69   $��14(�6K   
����  �$
��10,0q�6o   5�     5�     5�     