using ContextMenuToShowTopN_Example.nwindDataSetTableAdapters;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.PivotGrid;
using System.Data;
using System.Linq;
using System.Windows;

namespace ContextMenuToShowTopN_Example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        nwindDataSet.CustomerReportsDataTable table = new nwindDataSet.CustomerReportsDataTable();
        CustomerReportsTableAdapter tableAdapter = new CustomerReportsTableAdapter();
        public MainWindow()
        {
            InitializeComponent();
            tableAdapter.Fill(table);
            InitializePivot();
        }

        private void InitializePivot()
        {
            pivotGridControl1.DataSource = table;
            pivotGridControl1.RetrieveFields();
            pivotGridControl1.Fields["CompanyName"].Area = FieldArea.RowArea;
            pivotGridControl1.Fields["ProductName"].Area = FieldArea.RowArea;
            pivotGridControl1.Fields["ProductAmount"].Area = FieldArea.DataArea;
            pivotGridControl1.Fields["OrderDate"].Area = FieldArea.ColumnArea;
            pivotGridControl1.Fields["OrderDate"].GroupInterval = FieldGroupInterval.DateYear;
            pivotGridControl1.Fields["OrderDate"].DisplayFolder = "Date";
        }

        private void pivotGridControl1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            FieldValueElement fvElement = e.TargetElement as FieldValueElement;
            if (fvElement == null) return;

            FieldValueElementData fvElementData = fvElement.ElementData as FieldValueElementData;
            if (fvElementData.IsLastLevelItem)
            {
                string itemCaption = string.Format( "Top 5 Values in this {0}", fvElementData.IsColumn ? "Column" : "Row");
                BarButtonItem item = new BarButtonItem { Content = itemCaption };
                item.ItemClick += item_ItemClick;
                item.Tag = fvElementData;
                e.Customizations.Add(new AddBarItemAction { Item = item });
            }
        }

        void item_ItemClick(object sender, ItemClickEventArgs e)
        {
            FieldValueElementData elementData = e.Item.Tag as FieldValueElementData;
            elementData.PivotGrid.BeginUpdate();

            var sortConditions = elementData.PivotGrid.GetFieldsByArea(elementData.IsColumn ? FieldArea.ColumnArea : FieldArea.RowArea).
                Where(f => f.AreaIndex <= elementData.Field.AreaIndex).Select(f => new
            {
                Field = f,
                Value = elementData.PivotGrid.GetFieldValue(f, elementData.MaxIndex)
            });

            elementData.PivotGrid.GetFieldsByArea(elementData.IsColumn ? FieldArea.RowArea : FieldArea.ColumnArea).ForEach(f =>
            {
                f.SortOrder = FieldSortOrder.Descending;
                f.SortByField = elementData.DataField;
                f.SortByConditions.Clear();
                f.SortByConditions.AddRange(sortConditions.Select(c => new SortByCondition(c.Field, c.Value)));
                f.TopValueCount = 5;
                f.TopValueShowOthers = true;

            });
            elementData.PivotGrid.EndUpdate();
        }
    }
}
