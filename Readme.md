# How to Implement a Context Menu Action to Display Top 5 Values in a Column or Row


The key task is to enable the [Sorting by Summary](https://docs.devexpress.com/WPF/8072) and [Top N Values](https://docs.devexpress.com/WPF/8063) features simultaneously.

![screenshot](https://github.com/DevExpress-Examples/how-to-to-show-top-n-values-calculated-by-a-certain-measure-using-the-context-menu-t353100/blob/18.1.3%2B/images/screenshot.png)

API in this example:

* [PivotGridControl.PopupMenuShowing](https://docs.devexpress.com/WPF/DevExpress.Xpf.PivotGrid.PivotGridControl.PopupMenuShowing) event
* [PopupMenuShowingEventArgs.TargetElement](https://docs.devexpress.com/WPF/DevExpress.Xpf.PivotGrid.PopupMenuShowingEventArgs.TargetElement) property
* [FieldValueElement](https://docs.devexpress.com/WPF/DevExpress.Xpf.PivotGrid.FieldValueElement) class
* [FieldValueElementData](https://docs.devexpress.com/WPF/DevExpress.Xpf.PivotGrid.FieldValueElementData) class
* [PivotGridControl.GetFieldsByArea](https://docs.devexpress.com/WPF/DevExpress.Xpf.PivotGrid.PivotGridControl.GetFieldsByArea(DevExpress.Xpf.PivotGrid.FieldArea)) method
* [PivotGridControl.GetFieldValue](https://docs.devexpress.com/WPF/DevExpress.Xpf.PivotGrid.PivotGridControl.GetFieldValue.overloads) method
* [SortByConditionCollection](https://docs.devexpress.com/WPF/DevExpress.Xpf.PivotGrid.SortByConditionCollection) class
* [PivotGridField.SortByConditions](https://docs.devexpress.com/WPF/DevExpress.Xpf.PivotGrid.PivotGridField.SortByConditions) property
* [PivotGridField.SortByOrder](https://docs.devexpress.com/WPF/DevExpress.Xpf.PivotGrid.PivotGridField.SortByOrder) property
* [PivotGridField.SortByField](https://docs.devexpress.com/WPF/DevExpress.Xpf.PivotGrid.PivotGridField.SortByField) property
* [PivotGridField.TopValueCount](https://docs.devexpress.com/WPF/DevExpress.Xpf.PivotGrid.PivotGridField.TopValueCount) property 
* [PivotGridField.TopValueShowOthers](https://docs.devexpress.com/WPF/DevExpress.Xpf.PivotGrid.PivotGridField.TopValueShowOthers) property 