   M S B A M L   `   `   `  ����G  CWinePOSFinal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WinePOSFinalWinePOSFinal  5      '    !WinePOSFinal.InventoryMaintenance   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ,localclr-namespace:WinePOSFinal     5      .��5	      �   � RowDefinitions  5   
   �� 5      $��1* �6   6*   �� 5      $��4* �6   6*   �� 5      $��2* �6   6*   �� 5      $��3* �6   6*    � ColumnDefinitions 5   
   �� 5      .+�5   
   �� $��0��6   $��0��6"   .+�5      �� ���#FFFCF2
�������$��1q�6=   $��0,0,0,10q�6Q   ._�5      ��  �� Orientation$ Vertical=�6   $��5q�64   .+�5      �� $��Item: ��6    $��16(�6.   $
��Bold"�6<   
��� ���6N   5      5      5      �� 5   
   $��1��6   $��0��6"   .+�5      �  5      �� 5      $��4* �6'   �� 5       $��4* �6'   �� 5!      $��2* �6'     5#      �� 5$      $
��Auto �6$   .+�5(      �� $��0��6   $��0��6*   .+�5)      �  5*      �� 5+      $��4* �6/   �� 5,      $��6* �6/     5.      �� 5/      �� 50      �� 51      �� 52      �� 53      �� 54      .+�56      �� $ 
Horizontal=�6%   $��Right=�6>   $��0��6Z   $��0��6g   $	��0,5q�6w   .+�57      �� $��
Category: ��6(   $	��0,5q�6:   $
��Bold"�6G   58      �� 59      $ 
Horizontal=�6%   $
��Left=�6>   $��0��6Y   $��1��6f   $	��0,5q�6v   .+�5:      �� -   5        $��
cbCategory��5:   '   $	��150��69   $��25��6E   5;      5<      �� 5=      $ 
Horizontal=�6%   $��Right=�6>   $��1��6Z   $��0��6g   $	��0,5q�6w   .+�5>      �� $��UPC: ��6(   $	��0,5q�65   $
��Bold"�6B   5?      �� 5@      $ 
Horizontal=�6%   $��1��6>   $
��Left=�6K   $��1��6f   $	��0,5q�6v   .+�5A      �� -   5        $��txtUPC��5A   &   $	��150��64   $��25��6@   $��
4900005010��6L   5B      �� 5C      $ 
Horizontal=�6%   $��Right=�6>   $��2��6Z   $��0��6g   $	��0,5q�6w   .+�5D      �� $��Description: ��6(   $	��0,5q�6=   $
��Bold"�6J   5E      �� 5F      $ 
Horizontal=�6%   $��2��6>   $��1��6K   $
��Left=�6[   $	��0,5q�6v   .+�5G      �� -   5        $��txtDescription��5G   &   $	��150��6<   $��25��6H   $��
COKE 2 L T��6T   5H      �� 5I      $ 
Horizontal=�6%   $��Right=�6>   $��3��6Z   $��0��6g   $	��0,5q�6w   .+�5J      �� $��Additional Description: ��6(   $	��0,5q�6H   $
��Bold"�6U   5K      �� 5L      $ 
Horizontal=�6%   $��3��6>   $��1��6K   $
��Left=�6[   $	��0,5q�6v   .+�5M      �� -   5        $��txtADescription��5M   &   $	��150��6=   $��25��6I   5N      �� 5O      $ 
Horizontal=�6%   $��Right=�6>   $��4��6Z   $��0��6g   $	��0,5q�6w   .+�5P      �� $��Vendor Part No.: ��6(   $	��0,5q�6A   $
��Bold"�6N   5Q      �� 5R      $ 
Horizontal=�6%   $��4��6>   $��1��6K   $
��Left=�6[   $	��0,5q�6v   .+�5S      �� -   5        $��txtVendorPartNo��5S   &   $	��150��6=   $��25��6I   $��
COKE 2 L T��6U   5T      �� 5U      $ 
Horizontal=�6%   $��Right=�6>   $��5��6Z   $��0��6g   $	��0,5q�6w   .+�5V      �� $��Vendor Name: ��6(   $	��0,5q�6=   $
��Bold"�6J   5W      �� 5X      $ 
Horizontal=�6%   $��5��6>   $��1��6K   $
��Left=�6[   $	��0,5q�6v   .+�5Y      �� -   5        $��txtVendorName��5Y   &   $	��150��6;   $��25��6G   5Z      5[      5\      �� 5^      $��0��6   $��1��6*   .+�5_      �  5`      �� 5a      $��4* �6/   �� 5b      $��6* �6/     5d      �� 5e      �� 5f      �� 5g      �� 5h      �� 5i      �� 5j      .+�5l      �� $ 
Horizontal=�6%   $��Right=�6>   $��0��6Z   $��0��6g   $	��0,5q�6w   .+�5m      �� $��Item Cost($): ��6(   $	��0,5q�6>   $
��Bold"�6K   5n      �� 5o      $ 
Horizontal=�6%   $
��Left=�6>   $��0��6Y   $��1��6f   $	��0,5q�6v   .+�5p      �� -   5        $��txtItemCost��5p   &   $	��150��69   $��25��6E   $��
4900005010��6Q   5q      �� 5r      $ 
Horizontal=�6%   $��Right=�6>   $��1��6Z   $��0��6g   $	��0,5q�6w   .+�5s      �� $��Price You Charge($): ��6(   $	��0,5q�6E   $
��Bold"�6R   5t      �� 5u      $ 
Horizontal=�6%   $��1��6>   $
��Left=�6K   $��1��6f   $	��0,5q�6v   .+�5v      �� -   5        $��txtChargePrice��5v   &   $	��150��6<   $��25��6H   6T   5w      �� 5x      $ 
Horizontal=�6%   $��Right=�6>   $��2��6Z   $��0��6g   $	��0,5q�6w   .+�5y      �� $��Price With Sales Tax($): ��6(   $	��0,5q�6I   $
��Bold"�6V   ${�False��6h   5z      �� 5{      $ 
Horizontal=�6%   $��2��6>   $��1��6K   $
��Left=�6[   $	��0,5q�6v   .+�5|      �� -	   5        $��txtPriceWithTax��5|   &   $	��150��6=   $��25��6I   $��
COKE 2 L T��6U   5}      �� 5~      $ 
Horizontal=�6%   $��Right=�6>   $��3��6Z   $��0��6g   $	��0,5q�6w   .+�5      �� $��# In Stock: ��6(   $	��0,5q�6<   $
��Bold"�6I   5�      �� 5�      $ 
Horizontal=�6%   $��3��6>   $��1��6K   $
��Left=�6[   $	��0,5q�6v   .+�5�      �� -
   5        $��txtStock��5�   &   $	��150��66   $��25��6B   5�      �� 5�      $ 
Horizontal=�6%   $��Right=�6>   $��4��6Z   $��0��6g   $	��0,5q�6w   .+�5�      �� $��Case Cost: ��6(   $	��0,5q�6;   $
��Bold"�6H   5�      �� 5�      $ 
Horizontal=�6%   $��4��6>   $��1��6K   $
��Left=�6[   $	��0,5q�6v   .+�5�      �� -   5        $��txtCaseCost��5�   &   $	��150��69   $��25��6E   5�      �� 5�      $ 
Horizontal=�6%   $��Right=�6>   $��5��6Z   $��0��6g   $	��0,5q�6w   .+�5�      �� $��# In Case: ��6(   $	��0,5q�6;   $
��Bold"�6H   5�      �� 5�      $ 
Horizontal=�6%   $��5��6>   $��1��6K   $
��Left=�6[   $	��0,5q�6v   .+�5�      �� -   5        $��txtCase��5�   &   $	��150��65   $��25��6A   5�      5�      5�      �� 5�      $��0��6   $��2��6*   .+�5�      �� -   5        $��txtchkST��5�      $��	Sales Tax��6/   6^   �� 5�      -   5        $��	txtchkST2��5�      $��Sales Tax 2��60   6a   �� 5�      -   5        $��	txtchkST3��5�      $��Sales Tax 3��60   6a   �� 5�      -   5        $��	txtchkST4��5�      $��Sales Tax 4��60   6a   �� 5�      -   5        $��	txtchkST5��5�      $��Sales Tax 5��60   6a   �� 5�      -   5        $��	txtchkST6��5�      $��Sales Tax 6��60   6a   �� 5�      -   5        $��txtchkBT��5�      $��Bar Tax��6/   6\   �� 5�      -   5        $��txtQuickAdd��5�      $��QuickAdd��62   5�      5�      5�      �� 5!  
   $��3��6   $��0��6"   .+�5"     � $��10q�6     5#     �� 5$     $
��Auto �6$   �� 5%     $
��Auto �6$   .+�51     � $��1��6   $��Right=�6)   $	��Top=�6E   $��5q�6]   .+�53     �� -   5        $��btnSave��53     $��Save Button��6,   
������$	��120��6Z   $��35��6f   $��5q�6r   6}   �� 56     $��Delete��6   
�������$	��100��6F   $��35��6R   $��5q�6^   �� 57     -   5        $��btnClear��57     $��Clear��6-   
�������$	��100��6U   $��35��6a   $��5q�6m   6x   59     5:     5;     5<     5=     