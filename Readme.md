<!-- default file list -->
*Files to look at*:

* [MainWindow.xaml](./CS/WpfApplication31/MainWindow.xaml) (VB: [MainWindow.xaml.vb](./VB/WpfApplication31/MainWindow.xaml.vb))
* [MainWindow.xaml.cs](./CS/WpfApplication31/MainWindow.xaml.cs) (VB: [MainWindow.xaml.vb](./VB/WpfApplication31/MainWindow.xaml.vb))
<!-- default file list end -->
# How to to show Top N Values calculated by a certain measure using the Context Menu


To accomplish this task, it is necessary to enable the <a href="https://documentation.devexpress.com/WPF/CustomDocument8072.aspx">Sorting by Summary</a> and <a href="https://documentation.devexpress.com/WPF/CustomDocument8063.aspx">Top N Values</a> features simultaneously. To add a custom context menu item, handle the <a href="https://documentation.devexpress.com/#WPF/DevExpressXpfPivotGridPivotGridControl_PopupMenuShowingtopic">PivotGridControl.PopupMenuShowing</a> event. Please refer to the attached sample project demonstrating this approach. 

<br/>


