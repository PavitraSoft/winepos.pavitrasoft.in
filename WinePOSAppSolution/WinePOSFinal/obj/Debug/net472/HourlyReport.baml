   M S B A M L   `   `   `  ����G  CWinePOSFinal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WinePOSFinalWinePOSFinal  5   	       WinePOSFinal.HourlyReport   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:WinePOSFinal     5   	     � Title$  HourlyReport��5   	   $	��700��6   $	��400��6+   .��5	      � 
������� � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��HOURLY REPORT��6   $��20(�6)   $
��Bold"�67   $��Center=�6I   $
��0,10q�6f   $��0��6t   �� 5   
   $��Center=�6   $	��Top=�62   $��1��6J   $
��0,10q�6W   .+�5      �� -   5        $��txtDateFrom��5      $��1/3/2025 12:00:00 AM��6+   $��12(�6G    �� TextAlignment$ Center=�6U   �� 5      -   5        $��	txtDateTo��5      $��1/3/2025 11:59:59 PM��6)   $��12(�6E   $ Center=�6S   5      �� 5    
   $��1q�6   
������� �� CornerRadius$ 5��68   $��20,10q�6I   $��2��6X   ._�5!      &   System.Windows.Controls.DataGrid  -   5          Name$ dataGrid��5!         AutoGenerateColumns$
 True��5"      $��Stretch=�5#      $��Stretch=�5$         CanUserAddRows$ False��5%         
IsReadOnly$
 True��5&      5'      �� 5*   
    �� Orientation$ 
Horizontal=�6   $��Center=�6.   $��Bottom=�6K   $
��0,10q�6f   $��3��6t   .+�5+      �� -   5        $��PRINT��5+      $��80��6%   $��30��60   
��� ���$
��10,0q�6P   6^   �� 5,      -   5        $��EXPORT��5,      $��80��6&   $��30��61   
����ح�$
��10,0q�6T   6b   �� 5-      -   5        $
��EXIT��5-      $��80��6$   $��30��6/   
���  ��$
��10,0q�6L   
�������6m   5.      5/      50      