   M S B A M L   `   `   `  ����  FaceAPI-WPF-SamplesA    ;Microsoft.ProjectOxford.Face.Controls.FaceiconDetectionPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      -   5          N�Name$  FaceiconDetectionPane��5   7   $	��360��6V   $	��710��6f    N� FontSize$ 14(�6r   .��5      �  � ColumnDefinitions 5	   
   �� 5
      $	��300 �6   �� 5      $��96 �6   �� 5      $	��314 �6    � RowDefinitions 5   
   �� 5      $��54 �6   �� 5      $��45 �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��0��6!   $��3��61   $
��Left=�6E   $
��Wrap=�6�   ��6`   �� *FaceiconDetectionDescription+6�   � 5   
   $	��300��6   $	��300��6   $��0��6(   $��2��68   $	��Top=�6E    5      �� 5      $
��197* �6#   �� 5      $	��38* �6#   �� 5      $	��66* �6#   .+�5      �� $��Center=�6   $��Center=�68    K� ZIndex$	 999��6�   $��150,150,46.5,150q�69  ��6S   �� *ResultCollection+ �� ElementName$ FaceiconDetectionPane����6�   �� *MaxImageSizes+$ FaceiconDetectionPane����6�   �� *MaxImageSizes+$ FaceiconDetectionPane����5      �� 5      .R�5      �� 
������ $��Center=�6:   $��Center=�6W   ��6r   �� *MaxImageSizes+$ FaceiconDetectionPane����6�   �� *MaxImageSizes+$ FaceiconDetectionPane��5      ��5       �� 5!       �� 
TargetType" ��6   .�5"      ��  �� Property �@��Left �� Value 68   �� *Left+�� 5#       �@��Top 67   �� *Top+5$      ��5&      �� 5'      .R�5(      �� ���	#FF26B8F4$��1��6i   ��6$   �� *Width+��6<   �� *Height+6i   5)      5+      �� 5,      -   5        	 ��Name$	 ImageDisplay��5,   �   $��Center=�6   $��Center=�61   $��3��6�   ��6L   �� *MaxImageSizes+$ FaceiconDetectionPane����6�   �� *MaxImageSizes+$ FaceiconDetectionPane����5-      �� 5.      " ��6   .�5/      ��  � �� 63   �� *SelectedFile+$ FaceiconDetectionPane��
 �� Triggers
 50      �� 51       �� Binding 6*   �� *SelectedFile+$ FaceiconDetectionPane�� �� Value 6o   Z� .a�52   "   ��  � ��$7 1/FaceAPI-WPF-Samples;component/Assets/default.jpg��6;   53      55      57      58      �� 59   
   -   5        $��0��59      $��1��6!   $��Choose Image��6.   $	��300��6E   ���#2196f3
�������
������ $
��Left=�6�   $��35��6�   $	��Top=�6�   �� 5:   
   $��2��6   $��1��6$   $
��Left=�61   $	��Top=�6L   ��6d   �� *DetectedResultsInText+$ FaceiconDetectionPane���� 5;   
   -   5        $	 ImageiconDisplay��5;      $��2��6*   $
��Left=�6:   $	��261��6U   $��2��6b   $	��Top=�6o   $	��274��6�   5<      5=      