Imports ContextMenuToShowTopN_Example.nwindDataSetTableAdapters
Imports DevExpress.Xpf.Bars
Imports DevExpress.Xpf.PivotGrid
Imports DevExpress.Xpf.PivotGrid.Internal
Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Windows

Namespace ContextMenuToShowTopN_Example
	''' <summary>
	''' Interaction logic for MainWindow.xaml
	''' </summary>

	Partial Public Class MainWindow
		Inherits Window

		Private table As New nwindDataSet.ProductReportsDataTable()
		Private tableAdapter As New ProductReportsTableAdapter()
		Public Sub New()
			InitializeComponent()
			tableAdapter.Fill(table)
			InitializePivot()
		End Sub

		Private Sub InitializePivot()
			pivotGridControl1.DataSource = table
			pivotGridControl1.RetrieveFields()
			pivotGridControl1.Fields("CategoryName").Area = FieldArea.RowArea
			pivotGridControl1.Fields("ProductName").Area = FieldArea.RowArea
			pivotGridControl1.Fields("ProductSales").Area = FieldArea.DataArea
			pivotGridControl1.Fields("ShippedDate").Area = FieldArea.ColumnArea
			pivotGridControl1.Fields("ShippedDate").GroupInterval = FieldGroupInterval.DateMonthYear
			pivotGridControl1.Fields("ShippedDate").DisplayFolder = "Date"
			pivotGridControl1.BestFit()
		End Sub

		Private Sub pivotGridControl1_PopupMenuShowing(ByVal sender As Object, ByVal e As PopupMenuShowingEventArgs)
			Dim fvElement As FieldValueElement = TryCast(e.TargetElement, FieldValueElement)
			If fvElement Is Nothing Then
				Return
			End If

			Dim valueItem As FieldValueItem = TryCast(fvElement.ElementData, FieldValueItem)
			If valueItem.IsLastLevelItem Then
				Dim itemCaption As String = String.Format("Top 5 Values in this {0}",If(valueItem.IsColumn, "Column", "Row"))
				Dim item As BarCheckItem = New BarCheckItem With {.Content = itemCaption}
				If IsTopFiveValuesApplied(valueItem) Then
					item.IsChecked = True
				End If
				AddHandler item.CheckedChanged, AddressOf Item_CheckedChanged

				item.Tag = valueItem
				e.Customizations.Add(New AddBarItemAction With {.Item = item})
			End If
		End Sub

		Private Sub Item_CheckedChanged(ByVal sender As Object, ByVal e As ItemClickEventArgs)
			Dim item As BarCheckItem = TryCast(sender, BarCheckItem)
			Dim elementData As FieldValueItem = TryCast(e.Item.Tag, FieldValueItem)
			If CBool(item.IsChecked) Then
				SetTopFiveValues(elementData)
			Else
				ResetTopFiveValues(elementData.PivotGrid)
			End If
		End Sub
		Private Shared Sub SetTopFiveValues(ByVal valueItem As FieldValueItem)
			Dim sortConditions = GetConditions(valueItem)
			valueItem.PivotGrid.BeginUpdate()
			ResetTopFiveValues(valueItem.PivotGrid)
			valueItem.PivotGrid.GetFieldsByArea(If(valueItem.IsColumn, FieldArea.RowArea, FieldArea.ColumnArea)).ForEach(Sub(f)
				f.SortOrder = FieldSortOrder.Descending
				f.SortByField = valueItem.DataField
				f.SortByConditions.Clear()
				f.SortByConditions.AddRange(sortConditions.Select(Function(c) New SortByCondition(c.Key, c.Value)))
				f.TopValueCount = 5
				f.TopValueShowOthers = True
			End Sub)
			valueItem.PivotGrid.EndUpdate()
		End Sub
		Private Shared Function IsTopFiveValuesApplied(ByVal valueItem As FieldValueItem) As Boolean
			Dim fields = valueItem.PivotGrid.GetFieldsByArea(If(valueItem.IsColumn, FieldArea.RowArea, FieldArea.ColumnArea))
			If fields.Count = 0 Then
				Return False
			End If
			Dim conditions = GetConditions(valueItem)
			For Each f As PivotGridField In fields
				If f.TopValueCount <> 5 Then
					Return False
				End If
				If conditions.Count <> f.SortByConditions.Count Then
					Return False
				End If
				For i As Integer = 0 To conditions.Count - 1
					If f.SortByConditions(i).Field IsNot conditions(i).Key OrElse f.SortByConditions(i).Value IsNot conditions(i).Value Then
						Return False
					End If
				Next i
			Next f
			Return True
		End Function
		Private Shared Sub ResetTopFiveValues(ByVal pivotGrid As PivotGridControl)
			pivotGrid.BeginUpdate()
			Dim fields = pivotGrid.GetFieldsByArea(FieldArea.ColumnArea).Union(pivotGrid.GetFieldsByArea(FieldArea.RowArea))
			For Each f In fields
				f.SortByField = Nothing
				f.SortByConditions.Clear()
				f.TopValueCount = 0
				f.TopValueShowOthers = False
			Next f
			pivotGrid.EndUpdate()
		End Sub
		Private Shared Function GetConditions(ByVal valueItem As FieldValueItem) As List(Of KeyValuePair(Of PivotGridField, Object))
			Dim fields = valueItem.PivotGrid.GetFieldsByArea(If(valueItem.IsColumn, FieldArea.ColumnArea, FieldArea.RowArea)).Where(Function(f) f.AreaIndex <= valueItem.Field.AreaIndex)
			Return fields.Select(Function(f) New KeyValuePair(Of PivotGridField, Object)(f, valueItem.PivotGrid.GetFieldValue(f, valueItem.MinLastLevelIndex))).ToList()
		End Function


	End Class
End Namespace
