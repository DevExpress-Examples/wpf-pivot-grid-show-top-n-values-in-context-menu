Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.Data
Imports WpfApplication31.nwindDataSetTableAdapters
Imports DevExpress.Xpf.PivotGrid
Imports DevExpress.Xpf.PivotGrid.Internal
Imports DevExpress.Xpf.Bars

Namespace WpfApplication31
	''' <summary>
	''' Interaction logic for MainWindow.xaml
	''' </summary>

	Partial Public Class MainWindow
		Inherits Window

		Private table As New nwindDataSet.CustomerReportsDataTable()
		Private tableAdapter As New CustomerReportsTableAdapter()
		Public Sub New()
			InitializeComponent()
			tableAdapter.Fill(table)

			InitializePivot()


		End Sub

		Private Sub InitializePivot()
			pivotGridControl1.BeginUpdate()
			pivotGridControl1.DataSource = table
			pivotGridControl1.RetrieveFields()
			pivotGridControl1.Fields("CompanyName").Area = FieldArea.RowArea
			pivotGridControl1.Fields("ProductName").Area = FieldArea.RowArea
			pivotGridControl1.Fields("ProductAmount").Area = FieldArea.DataArea
			pivotGridControl1.Fields("OrderDate").Area = FieldArea.ColumnArea
			pivotGridControl1.Fields("OrderDate").GroupInterval = FieldGroupInterval.DateYear
			pivotGridControl1.EndUpdate()

		End Sub

		Private Sub BarButtonItem_ItemClick(ByVal sender As Object, ByVal e As DevExpress.Xpf.Bars.ItemClickEventArgs)
			Dim menuInfo As PivotGridFieldValueMenuInfo = TryCast(pivotGridControl1.GridMenu.MenuInfo, PivotGridFieldValueMenuInfo)
			Me.Title = menuInfo.ValueItem.DisplayText

		End Sub

		Private Sub pivotGridControl1_PopupMenuShowing(ByVal sender As Object, ByVal e As PopupMenuShowingEventArgs)
            Dim fvElement As FieldValueElement = TryCast(e.TargetElement, FieldValueElement)
            Dim fvItem As FieldValueItem = fvElement.Item
			If fvItem.IsLastLevelItem Then
				Dim itemCaption As String = String.Format("Top 5 Values in this {0}",If(fvItem.IsColumn, "Column", "Row"))
				Dim item As BarButtonItem = New BarButtonItem With {.Content = itemCaption}
				AddHandler item.ItemClick, AddressOf item_ItemClick
				item.Tag = fvItem
				e.Customizations.Add(New AddBarItemAction With {.Item = item})
			End If
			'cellElement.

		End Sub

		Private Sub item_ItemClick(ByVal sender As Object, ByVal e As ItemClickEventArgs)
			Dim valueItem As FieldValueItem = TryCast(e.Item.Tag, FieldValueItem)
			valueItem.PivotGrid.BeginUpdate()

			Dim sortConditions = valueItem.PivotGrid.GetFieldsByArea(If(valueItem.IsColumn, FieldArea.ColumnArea, FieldArea.RowArea)).Where(Function(f) f.AreaIndex <= valueItem.Field.AreaIndex).Select(Function(f) New With {Key .Field = f, Key .Value = valueItem.PivotGrid.GetFieldValue(f, valueItem.MaxLastLevelIndex)})

			valueItem.PivotGrid.GetFieldsByArea(If(valueItem.IsColumn, FieldArea.RowArea, FieldArea.ColumnArea)).ForEach(Sub(f)
				f.SortOrder = FieldSortOrder.Descending
				f.SortByField = valueItem.DataField
				f.SortByConditions.Clear()
				f.SortByConditions.AddRange(sortConditions.Select(Function(c) New SortByCondition(c.Field, c.Value)))
				f.TopValueCount = 5
				f.TopValueShowOthers = True
			End Sub)
			valueItem.PivotGrid.EndUpdate()
		End Sub
	End Class
End Namespace
