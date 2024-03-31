namespace YouthUnion
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Microsoft.Office.Interop.Excel;

    using Application = Microsoft.Office.Interop.Excel.Application;
    using DataTable = System.Data.DataTable;
    using Window = System.Windows.Window;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var participants = ParticipantsComboBox.ItemsSource.Cast<Participant>().ToList();
            participants.Insert(0, new Participant { FullName = "По ответственному" });
            ParticipantsComboBox.ItemsSource = participants;
            ParticipantsComboBox.SelectedIndex = 0;
        }

        public static DataTable DataGridToDataTable(DataGrid dg)
        {
            dg.SelectAllCells();
            dg.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dg);
            dg.UnselectAllCells();
            var result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            var lines = result.Split(new[] { "\r\n", "\n" },
                StringSplitOptions.None);
            var fields = lines[0].Split(new[] { ',' });
            var cols = fields.GetLength(0);
            var dt = new DataTable();

            for (var i = 0; i < cols; i++)
                dt.Columns.Add(fields[i].ToUpper(), typeof(string));
            for (var i = 1; i < lines.GetLength(0) - 1; i++)
            {
                fields = lines[i].Split(new[] { ',' });
                var row = dt.NewRow();
                for (var f = 0; f < cols; f++)
                    row[f] = fields[f];
                dt.Rows.Add(row);
            }

            return dt;
        }

        private static void ExportToExcel(DataTable dataTable)
        {
            var excel = new Application();
            var wb = excel.Workbooks.Add();
            var ws = (Worksheet)wb.ActiveSheet;
            ws.Columns.AutoFit();
            ws.Columns.EntireColumn.ColumnWidth = 25;

            // Header row
            for (var i = 0; i < dataTable.Columns.Count; i++)
                ws.Range["A1"].Offset[0, i].Value = dataTable.Columns[i].ColumnName;

            // Data Rows
            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                ws.Range["A2"].Offset[i].Resize[1, dataTable.Columns.Count].Value =
                    dataTable.Rows[i].ItemArray;
            }

            excel.Visible = true;
            wb.Activate();
        }


        private void GenerateReportButtonClick(object sender, RoutedEventArgs e)
        {
            var dataTable = DataGridToDataTable(EventsDataGrid);
            ExportToExcel(dataTable);
        }


        private DataGrid? GetDataGridFromText(string selectedItem)
        {
            return selectedItem switch
            {
                "Сотрудники" => ParticipantDataGrid,
                "Мероприятия" => EventsDataGrid,
                "Назначения" => AssignmentDataGrid,
                _ => null
            };
        }

        private void OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs eventArgs)
        {
            DataGridHelper.OnAutoGeneratingColumn(sender, eventArgs);
        }

        private void OnComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = (ComboBox)sender;
            var selectedItem = (ComboBoxItem)combobox.SelectedItem;

            foreach (var comboBoxItem in combobox.Items.Cast<ComboBoxItem>())
            {
                var dataGrid = GetDataGridFromText(comboBoxItem.Content.ToString());
                if (dataGrid == null)
                    continue;

                if (selectedItem == comboBoxItem)
                {
                    dataGrid.Visibility = Visibility.Visible;
                    continue;
                }

                dataGrid.Visibility = Visibility.Collapsed;
            }
        }
    }
}