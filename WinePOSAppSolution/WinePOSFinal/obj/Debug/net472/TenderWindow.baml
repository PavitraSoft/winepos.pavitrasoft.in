   M S B A M L   `   `   `  ����G  CWinePOSFinal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WinePOSFinalWinePOSFinal  5   	       WinePOSFinal.TenderWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:WinePOSFinal     5   	     � Title$  TenderWindow��5   	   $	��600��6   $
��1100��6+   .��5	      � $��10q�6   .+�5   
   �� -   5         ��Name$ AmountTextBox��5      $��24(�6)   $	��250��67   $��50��6C   $
��Left=�6O   $	��Top=�6j   $��10q�6�   
�������
���   �5   8   A� 5   
    A� Rows$ 4��6    A� Columns$ 3��6   $	��250��6+   $	��300��67   $
��Left=�6D   $	��Top=�6_   $��	10,70,0,0q�6w   .+�5      �� -   5        $��7��5      
����İ�$��20(�6=   6K   �� 5      -   5        $��8��5      
����İ�$��20(�6=   6K   �� 5      -   5        $��9��5      
����İ�$��20(�6=   6K   �� 5      -   5        $��4��5      
����İ�$��20(�6=   6K   �� 5      -   5        $��5��5      
����İ�$��20(�6=   6K   �� 5      -   5        $��6��5      
����İ�$��20(�6=   6K   �� 5      -   5        $��1��5      
����İ�$��20(�6=   6K   �� 5      -	   5        $��2��5      
����İ�$��20(�6=   6K   �� 5      -
   5        $��3��5      
����İ�$��20(�6=   6K   �� 5      -   5        $��+��5      
������$��20(�69   6G   �� 5      -   5        $��0��5      
����İ�$��20(�6=   6K   �� 5      -   5        $��-��5      
���Gc��$��20(�65   6C   �� 5      -   5        $	��←��5      $��3��6!   $��1��6.   
�������$��20(�5   ,   $
��Bold"�6:   �� 5      -   5        $��.��5      $��3��6!   $��2��6.   6>   5       �� 5#   
   -   5         ��Name$ RemainingAmountText��5#      $��20(�61   $��Right=�6?   $	��Top=�6[   $��	0,10,10,0q�6s   $��Amount Remaining: $0.00��5$      
��� � �63   &   System.Windows.Controls.DataGrid  5%   
   -   5          Name$ PaymentGrid��5%         AutoGenerateColumns$
 True��6(   $	��350��6C   $	��200��6O   $��Right=�6\   $	��Top=�5&      $��	0,50,10,0q�6+   
�������
���   �   
IsReadOnly$
 True��6d   �� 5)   
    �� Orientation$ 
Horizontal=�6   $��Center=�6.   $��Bottom=�6K   $��10q�6f   .+�5*      �� -   5        $��$1��5*      
��� � �$��20(�65   
�������$��70��6V   $��50��6a   $��5q�6m   6x   �� 5+      -   5        $��$2��5+      
��� � �$��20(�65   
�������$��70��6V   $��50��6a   $��5q�6m   6x   �� 5,      -   5        $��$5��5,      
��� � �$��20(�65   
�������$��70��6V   $��50��6a   $��5q�6m   6x   �� 5-      -   5        $	��$10��5-      
��� � �$��20(�66   
�������$��70��6W   $��50��6b   $��5q�6n   6y   �� 5.      -   5        $	��$20��5.      
��� � �$��20(�66   
�������$��70��6W   $��50��6b   $��5q�6n   6y   �� 5/      -   5        $	��$50��5/      
��� � �$��20(�66   
�������$��70��6W   $��50��6b   $��5q�6n   6y   �� 50      -   5        $��btnCustomAmount��50   <   $	��$50��6   
���2͚�$��20(�6S   
�������$��70��6t   $��50��6   $��5q�6�   6�   51      �� 54   
   $ 
Horizontal=�6   $��Right=�6.   $��Bottom=�6J   $��10q�6e   .+�55      �� -   5        $��Clear��55      
���Gc��$��20(�69   
�������$	��100��6Z   $��50��6f   $��5q�6r   6}   �� 56      -   5        $
��Done��56      
����  �$��20(�66   
�������$	��100��6W   $��50��6c   $��5q�6o   6z   57      58      59      