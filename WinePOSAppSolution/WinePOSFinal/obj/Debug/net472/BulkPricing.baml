   M S B A M L   `   `   `  ����G  CWinePOSFinal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WinePOSFinalWinePOSFinal  5   	       WinePOSFinal.BulkPricing   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:WinePOSFinal     5   	     � Title$  BulkPricing��5   	   $	��200��6   $	��300��6*   .��5	      � $��10q�6    � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��	Quantity:��6   $��Center=�6%   $��5q�6@   �� 5   
   -   5        $��txtQuantity��5      $��1��6%   $��5q�65   $	��150��6@   5      �� 5   
   $��Price:��6   $��Center=�6"   $��1��6=   $��5q�6J   �� 5   
   -   5        $��txtPrice��5      $��1��6"   $��1��62   $��5q�6?   $	��150��6J   5      �� 5   
   -   5        $	��Add��5      $��2��6   $��2��6,   $��80��6@   $��Center=�6K   $��5q�6h   6s   5       5!      