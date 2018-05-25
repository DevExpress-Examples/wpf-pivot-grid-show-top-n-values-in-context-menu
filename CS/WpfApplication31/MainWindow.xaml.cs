using DevExpress.Xpf.Bars;
using DevExpress.Xpf.PivotGrid;
using DevExpress.Xpf.PivotGrid.Internal;
using System.Data;
using System.Linq;
using System.Windows;
using WpfApplication31.nwindDataSetTableAdapters;

namespace WpfApplication31
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
            pivotGridControl1.BeginUpdate();
            pivotGridControl1.DataSource = table;
            pivotGridControl1.RetrieveFields();
            pivotGridControl1.Fields["CompanyName"].Area = FieldArea.RowArea;
            pivotGridControl1.Fields["ProductName"].Area = FieldArea.RowArea;
            pivotGridControl1.Fields["ProductAmount"].Area = FieldArea.DataArea;
            pivotGridControl1.Fields["OrderDate"].Area = FieldArea.ColumnArea;
            pivotGridControl1.Fields["OrderDate"].GroupInterval = FieldGroupInterval.DateYear;
            pivotGridControl1.EndUpdate();

        }

        private void BarButtonItem_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            PivotGridFieldValueMenuInfo menuInfo = pivotGridControl1.GridMenu.MenuInfo as PivotGridFieldValueMenuInfo;
            this.Title = menuInfo.ValueItem.DisplayText;

        }

        private void pivotGridControl1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            FieldValueElement fvElement = e.TargetElement as FieldValueElement;
            FieldValueItem fvItem = fvElement.Item;
            if (fvItem.IsLastLevelItem)
            {
                string itemCaption = string.Format( "Top 5 Values in this {0}", fvItem.IsColumn ? "Column" : "Row");
                BarButtonItem item = new BarButtonItem { Content = itemCaption };
                item.ItemClick += item_ItemClick;
                item.Tag = fvItem;
                e.Customizations.Add(new AddBarItemAction { Item = item });
            }
        }

        void item_ItemClick(object sender, ItemClickEventArgs e)
        {
            FieldValueItem valueItem = e.Item.Tag as FieldValueItem;
            valueItem.PivotGrid.BeginUpdate();

            var sortConditions = valueItem.PivotGrid.GetFieldsByArea(valueItem.IsColumn ? FieldArea.ColumnArea : FieldArea.RowArea).
                Where(f => f.AreaIndex <= valueItem.Field.AreaIndex).Select(f => new
            {
                Field = f,
                Value = valueItem.PivotGrid.GetFieldValue(f, valueItem.MaxLastLevelIndex)
            });

            valueItem.PivotGrid.GetFieldsByArea(valueItem.IsColumn ? FieldArea.RowArea : FieldArea.ColumnArea).ForEach(f =>
            {
                f.SortOrder = FieldSortOrder.Descending;
                f.SortByField = valueItem.DataField;
                f.SortByConditions.Clear();
                f.SortByConditions.AddRange(sortConditions.Select(c => new SortByCondition(c.Field, c.Value)));
                f.TopValueCount = 5;
                f.TopValueShowOthers = true;

            });
            valueItem.PivotGrid.EndUpdate();
        }
    }
}
