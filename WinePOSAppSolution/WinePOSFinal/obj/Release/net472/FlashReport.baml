   M S B A M L   `   `   `  ���� E  AOnestopPOS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WinePOSFinalWinePOSFinal      WinePOSFinal.FlashReport   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ,localclr-namespace:WinePOSFinal       � Title$  FlashReport��$	��700��$	��400��.��� 
������� � RowDefinitions �� $
��Auto ��� $
��Auto ��� $��* ��� $��* ��� $��* ��� $
��Auto �.+��� $��FLASH REPORT��$��20(�$
��Bold"�$��Center=�$
��0,10q�$��0���� $��Center=�$	��Top=�$��1��$
��0,10q�.+��� -   $��txtDateFrom��$��1/3/2025 12:00:00 AM��$��12(� �� TextAlignment$ Center=��� -   $��	txtDateTo��$��1/3/2025 11:59:59 PM��$��12(�$ Center=��� $��1q�
������� �� CornerRadius$ 5��$��20,10q�$��2��._��� $��10q�.+��� $��SALES TOTALS��$
��Bold"�$ Center=�$��0,0,0,5q��  �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto � � ColumnDefinitions �� $��* ��� $
��Auto �.+��� $��	Net Sales��$��0��$��0���� -   $��NetSales��$��	$1,072.33��$��0��$��1��$ Right=��� $��Net Sales - Taxed��$��1��$��0���� -   $��	NetSalesT��$��	$1,014.04��$��1��$��1��$ Right=��� $��Net Sales - NOT Taxed��$��2��$��0���� -   $��
NetSalesNT��$��$58.29��$��2��$��1��$ Right=��� $��Taxes��$��5��$��0���� -   $	��Tax��$��$67.46��$��5��$��1��$ Right=��� $��Gross Sales��$��6��$��0���� -   $��
GrossSales��$��	$1,139.79��$��6��$��1��$ Right=��� $��1q�
�������$ 5��$��20,10q�$��3��._��� $��10q�.+��� $��PAYMENT TYPE BREAKDOWN��$
��Bold"�$ Center=�$��0,0,0,5q��  �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto � �� $��* ��� $
��Auto �.+��� $
��Cash��$��0��$��0���� -   $��txtCash��$��$531.95��$��0��$��1��$ Right=��� $��Checks��$��1��$��0���� -	   $��	txtChecks��$��$607.84��$��1��$��1��$ Right=��� $��Credit/Debit��$��2��$��0���� -
   $��	txtCredit��$��$0.00��$��2��$��1��$ Right=��� $��PalmPAY��$��3��$��0���� -   $��
txtPalmPay��$��$0.00��$��3��$��1��$ Right=��� $��1q�
�������$ 5��$��20,10q�$��4��._��� $��10q�.+��� $��TRANSACTION STATISTICS��$
��Bold"�$ Center=�$��0,0,0,5q��  �� $
��Auto ��� $
��Auto � �� $��* ��� $
��Auto �.+��� $��Transactions��$��0��$��0���� -   $��txtTransactions��$��$531.95��$��0��$��1��$ Right=��� $��Average Transaction��$��1��$��0���� -   $��txtAvgTransactions��$��$531.95��$��1��$��1��$ Right=���  �� Orientation$ 
Horizontal=�$��Center=�$��Bottom=�$
��0,10q�$��5��.+��� -   $��PRINT��$��80��$��30��
��� ���$
��10,0q��� -   $��EXPORT��$��80��$��30��
����ح�$
��10,0q��� -   $
��EXIT��$��80��$��30��
���  ��$
��10,0q�
�������