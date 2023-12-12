Imports DevExpress.Xpf.Bars
Imports DevExpress.Xpf.PivotGrid
Imports DevExpress.Xpf.PivotGrid.Internal
Imports System.Data
Imports System.Linq
Imports System.Windows
Imports WpfApplication31.nwindDataSetTableAdapters

Namespace WpfApplication31

    ''' <summary>
    ''' Interaction logic for MainWindow.xaml
    ''' </summary>
    Public Partial Class MainWindow
        Inherits System.Windows.Window

        Private table As WpfApplication31.nwindDataSet.CustomerReportsDataTable = New WpfApplication31.nwindDataSet.CustomerReportsDataTable()

        Private tableAdapter As WpfApplication31.nwindDataSetTableAdapters.CustomerReportsTableAdapter = New WpfApplication31.nwindDataSetTableAdapters.CustomerReportsTableAdapter()

        Public Sub New()
            Me.InitializeComponent()
            Me.tableAdapter.Fill(Me.table)
            Me.InitializePivot()
        End Sub

        Private Sub InitializePivot()
            Me.pivotGridControl1.DataSource = Me.table
            Me.pivotGridControl1.RetrieveFields()
            Me.pivotGridControl1.Fields(CStr(("CompanyName"))).Area = DevExpress.Xpf.PivotGrid.FieldArea.RowArea
            Me.pivotGridControl1.Fields(CStr(("ProductName"))).Area = DevExpress.Xpf.PivotGrid.FieldArea.RowArea
            Me.pivotGridControl1.Fields(CStr(("ProductAmount"))).Area = DevExpress.Xpf.PivotGrid.FieldArea.DataArea
            Me.pivotGridControl1.Fields(CStr(("OrderDate"))).Area = DevExpress.Xpf.PivotGrid.FieldArea.ColumnArea
            Me.pivotGridControl1.Fields(CStr(("OrderDate"))).GroupInterval = DevExpress.Xpf.PivotGrid.FieldGroupInterval.DateYear
        End Sub

        Private Sub BarButtonItem_ItemClick(ByVal sender As Object, ByVal e As DevExpress.Xpf.Bars.ItemClickEventArgs)
            Dim menuInfo As DevExpress.Xpf.PivotGrid.PivotGridFieldValueMenuInfo = TryCast(Me.pivotGridControl1.GridMenu.MenuInfo, DevExpress.Xpf.PivotGrid.PivotGridFieldValueMenuInfo)
            Me.Title = menuInfo.ValueItem.DisplayText
        End Sub

        Private Sub pivotGridControl1_PopupMenuShowing(ByVal sender As Object, ByVal e As DevExpress.Xpf.PivotGrid.PopupMenuShowingEventArgs)
            Dim fvElement As DevExpress.Xpf.PivotGrid.FieldValueElement = TryCast(e.TargetElement, DevExpress.Xpf.PivotGrid.FieldValueElement)
            Dim fvItem As DevExpress.Xpf.PivotGrid.Internal.FieldValueItem = fvElement.Item
            If fvItem.IsLastLevelItem Then
                Dim itemCaption As String = String.Format("Top 5 Values in this {0}", If(fvItem.IsColumn, "Column", "Row"))
                Dim item As DevExpress.Xpf.Bars.BarButtonItem = New DevExpress.Xpf.Bars.BarButtonItem With {.Content = itemCaption}
                AddHandler item.ItemClick, AddressOf Me.item_ItemClick
                item.Tag = fvItem
                e.Customizations.Add(New DevExpress.Xpf.Bars.AddBarItemAction With {.Item = item})
            End If
        End Sub

        Private Sub item_ItemClick(ByVal sender As Object, ByVal e As DevExpress.Xpf.Bars.ItemClickEventArgs)
            Dim valueItem As DevExpress.Xpf.PivotGrid.Internal.FieldValueItem = TryCast(e.Item.Tag, DevExpress.Xpf.PivotGrid.Internal.FieldValueItem)
            valueItem.PivotGrid.BeginUpdate()
            Dim sortConditions = valueItem.PivotGrid.GetFieldsByArea(If(valueItem.IsColumn, DevExpress.Xpf.PivotGrid.FieldArea.ColumnArea, DevExpress.Xpf.PivotGrid.FieldArea.RowArea)).Where(Function(f) f.AreaIndex <= valueItem.Field.AreaIndex).[Select](Function(f) New With {.Field = f, .Value = valueItem.PivotGrid.GetFieldValue(f, valueItem.MaxLastLevelIndex)})
            valueItem.PivotGrid.GetFieldsByArea(CType((If(valueItem.IsColumn, DevExpress.Xpf.PivotGrid.FieldArea.RowArea, DevExpress.Xpf.PivotGrid.FieldArea.ColumnArea)), DevExpress.Xpf.PivotGrid.FieldArea)).ForEach(Sub(f)
                f.SortOrder = DevExpress.Xpf.PivotGrid.FieldSortOrder.Descending
                f.SortByField = valueItem.DataField
                f.SortByConditions.Clear()
                f.SortByConditions.AddRange(sortConditions.[Select](Function(c) New DevExpress.Xpf.PivotGrid.SortByCondition(c.Field, c.Value)))
                f.TopValueCount = 5
                f.TopValueShowOthers = True
            End Sub)
            valueItem.PivotGrid.EndUpdate()
        End Sub
    End Class
End Namespace
