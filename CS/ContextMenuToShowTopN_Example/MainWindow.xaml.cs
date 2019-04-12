using DevExpress.DataAccess.Excel;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.PivotGrid;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;

namespace ContextMenuToShowTopN_Example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : DevExpress.Xpf.Core.ThemedWindow
    {
        ExcelDataSource xlDataSource = new ExcelDataSource();
        public MainWindow()
        {
            InitializeComponent();
            InitializeExcelDataSource();
            InitializePivot();
        }

        private void InitializeExcelDataSource()
        {
            xlDataSource.FileName = "SalesPerson.xlsx";
            ExcelWorksheetSettings worksheetSettings = new ExcelWorksheetSettings("Data");
            xlDataSource.SourceOptions = new ExcelSourceOptions(worksheetSettings);
            xlDataSource.SourceOptions.SkipEmptyRows = false;
            xlDataSource.SourceOptions.UseFirstRowAsHeader = true;
            xlDataSource.Fill();
        }

        private void InitializePivot()
        {
            pivotGridControl1.DataSource = xlDataSource;
            pivotGridControl1.RetrieveFields();
            pivotGridControl1.Fields["Sales Person"].Area = FieldArea.RowArea;
            pivotGridControl1.Fields["CategoryName"].Area = FieldArea.RowArea;
            pivotGridControl1.Fields["Quantity"].Area = FieldArea.DataArea;
            pivotGridControl1.Fields["OrderDate"].Area = FieldArea.ColumnArea;
            pivotGridControl1.Fields["OrderDate"].GroupInterval = FieldGroupInterval.DateYear;
        }

        private void pivotGridControl1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            FieldValueElement fvElement = e.TargetElement as FieldValueElement;
            if (fvElement == null) return;

            FieldValueElementData valueItem = fvElement.ElementData as FieldValueElementData;
            if (valueItem.IsLastLevelItem)
            {
                string itemCaption = string.Format("Top 5 Values in this {0}", valueItem.IsColumn ? "Column" : "Row");
                BarCheckItem item = new BarCheckItem { Content = itemCaption };
                if (IsTopFiveValuesApplied(valueItem))
                    item.IsChecked = true;
                item.CheckedChanged += Item_CheckedChanged;

                item.Tag = valueItem;
                e.Customizations.Add(new AddBarItemAction { Item = item });
            }
        }

        private void Item_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            BarCheckItem item = sender as BarCheckItem;
            FieldValueElementData elementData = e.Item.Tag as FieldValueElementData;
            if ((bool)item.IsChecked)
                SetTopFiveValues(elementData);
            else
                ResetTopFiveValues(elementData.PivotGrid);
        }
        private static void SetTopFiveValues(FieldValueElementData valueItem)
        {
            var sortConditions = GetConditions(valueItem);
            valueItem.PivotGrid.BeginUpdate();
            ResetTopFiveValues(valueItem.PivotGrid);
            valueItem.PivotGrid.GetFieldsByArea(valueItem.IsColumn ? FieldArea.RowArea : FieldArea.ColumnArea).ForEach(f => {
                f.SortOrder = FieldSortOrder.Descending;
                f.SortByField = valueItem.DataField;
                f.SortByConditions.Clear();
                f.SortByConditions.AddRange(sortConditions.Select(c => new SortByCondition(c.Key, c.Value)));
                f.TopValueCount = 5;
                f.TopValueShowOthers = true;
            });
            valueItem.PivotGrid.EndUpdate();
        }
        private static bool IsTopFiveValuesApplied(FieldValueElementData valueItem)
        {
            var fields = valueItem.PivotGrid.GetFieldsByArea(valueItem.IsColumn ? FieldArea.RowArea : FieldArea.ColumnArea);
            if (fields.Count == 0)
                return false;
            var conditions = GetConditions(valueItem);
            foreach (PivotGridField f in fields)
            {
                if (f.TopValueCount != 5)
                    return false;
                if (conditions.Count != f.SortByConditions.Count)
                    return false;
                for (int i = 0; i < conditions.Count; i++)
                {
                    if (f.SortByConditions[i].Field != conditions[i].Key ||
                        f.SortByConditions[i].Value != conditions[i].Value)
                        return false;
                }
            }
            return true;
        }
        private static void ResetTopFiveValues(PivotGridControl pivotGrid)
        {
            pivotGrid.BeginUpdate();
            var fields = pivotGrid.GetFieldsByArea(FieldArea.ColumnArea).Union(pivotGrid.GetFieldsByArea(FieldArea.RowArea));
            foreach (var f in fields)
            {
                f.SortByField = null;
                f.SortByConditions.Clear();
                f.TopValueCount = 0;
                f.TopValueShowOthers = false;
            }
            pivotGrid.EndUpdate();
        }
        private static List<KeyValuePair<PivotGridField, object>> GetConditions(FieldValueElementData valueItem)
        {
            var fields = valueItem.PivotGrid.GetFieldsByArea(valueItem.IsColumn ? FieldArea.ColumnArea : FieldArea.RowArea).Where(f => f.AreaIndex <= valueItem.Field.AreaIndex);
            return fields.
                Select(f => new KeyValuePair<PivotGridField, object>(f,
                    valueItem.PivotGrid.GetFieldValue(f, valueItem.MinIndex)
                )).ToList();
        }

        private void PivotGridControl1_Loaded(object sender, RoutedEventArgs e)
        {
            pivotGridControl1.BestFit(FieldArea.ColumnArea);
        }
    }
}
