Imports ContextMenuToShowTopN_Example.nwindDataSetTableAdapters
Imports DevExpress.Xpf.Bars
Imports DevExpress.Xpf.PivotGrid
Imports System.Data
Imports System.Linq
Imports System.Windows

Namespace ContextMenuToShowTopN_Example
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
            pivotGridControl1.DataSource = table
            pivotGridControl1.RetrieveFields()
            pivotGridControl1.Fields("CompanyName").Area = FieldArea.RowArea
            pivotGridControl1.Fields("ProductName").Area = FieldArea.RowArea
            pivotGridControl1.Fields("ProductAmount").Area = FieldArea.DataArea
            pivotGridControl1.Fields("OrderDate").Area = FieldArea.ColumnArea
            pivotGridControl1.Fields("OrderDate").GroupInterval = FieldGroupInterval.DateYear
            pivotGridControl1.Fields("OrderDate").DisplayFolder = "Date"
        End Sub

        Private Sub pivotGridControl1_PopupMenuShowing(ByVal sender As Object, ByVal e As PopupMenuShowingEventArgs)
            Dim fvElement As FieldValueElement = TryCast(e.TargetElement, FieldValueElement)
            If fvElement Is Nothing Then
                Return
            End If

            Dim fvElementData As FieldValueElementData = TryCast(fvElement.ElementData, FieldValueElementData)
            If fvElementData.IsLastLevelItem Then
                Dim itemCaption As String = String.Format("Top 5 Values in this {0}",If(fvElementData.IsColumn, "Column", "Row"))
                Dim item As BarButtonItem = New BarButtonItem With {.Content = itemCaption}
                AddHandler item.ItemClick, AddressOf item_ItemClick
                item.Tag = fvElementData
                e.Customizations.Add(New AddBarItemAction With {.Item = item})
            End If
        End Sub

        Private Sub item_ItemClick(ByVal sender As Object, ByVal e As ItemClickEventArgs)
            Dim elementData As FieldValueElementData = TryCast(e.Item.Tag, FieldValueElementData)
            elementData.PivotGrid.BeginUpdate()

            Dim sortConditions = elementData.PivotGrid.GetFieldsByArea(If(elementData.IsColumn, FieldArea.ColumnArea, FieldArea.RowArea)).Where(Function(f) f.AreaIndex <= elementData.Field.AreaIndex).Select(Function(f) New With { _
                Key .Field = f, _
                Key .Value = elementData.PivotGrid.GetFieldValue(f, elementData.MaxIndex) _
            })

            elementData.PivotGrid.GetFieldsByArea(If(elementData.IsColumn, FieldArea.RowArea, FieldArea.ColumnArea)).ForEach(Sub(f)
                f.SortOrder = FieldSortOrder.Descending
                f.SortByField = elementData.DataField
                f.SortByConditions.Clear()
                f.SortByConditions.AddRange(sortConditions.Select(Function(c) New SortByCondition(c.Field, c.Value)))
                f.TopValueCount = 5
                f.TopValueShowOthers = True

            End Sub)
            elementData.PivotGrid.EndUpdate()
        End Sub
    End Class
End Namespace
