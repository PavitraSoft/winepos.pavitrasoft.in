   M S B A M L   `   `   `  ���� E  AOnestopPOS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WinePOSFinalWinePOSFinal      WinePOSFinal.HourlyReport   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ,localclr-namespace:WinePOSFinal       � Title$  HourlyReport��$	��700��$	��400��.��� 
������� � RowDefinitions �� $
��Auto ��� $
��Auto ��� $��* ��� $
��Auto �.+��� $��HOURLY REPORT��$��20(�$
��Bold"�$��Center=�$
��0,10q�$��0���� $��Center=�$	��Top=�$��1��$
��0,10q�.+��� -   $��txtDateFrom��$��1/3/2025 12:00:00 AM��$��12(� �� TextAlignment$ Center=��� -   $��	txtDateTo��$��1/3/2025 11:59:59 PM��$��12(�$ Center=��� $��1q�
������� �� CornerRadius$ 5��$��20,10q�$��2��._�&   System.Windows.Controls.DataGrid  -     Name$ dataGrid��   AutoGenerateColumns$
 True��$��Stretch=�$��Stretch=�   CanUserAddRows$ False��   
IsReadOnly$
 True����  �� Orientation$ 
Horizontal=�$��Center=�$��Bottom=�$
��0,10q�$��3��.+��� -   $��PRINT��$��80��$��30��
��� ���$
��10,0q��� -   $��EXPORT��$��80��$��30��
����ح�$
��10,0q��� -   $
��EXIT��$��80��$��30��
���  ��$
��10,0q�
�������