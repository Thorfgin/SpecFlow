using GemBox.Spreadsheet;
using GemBox.Spreadsheet.Tables;
using Gherkin.Ast;
using System.Collections.Generic;
using System.Linq;

namespace SpecFlow.Excel.SpecFlowPlugin
{
    class ExcelTableRows
    {
        public TableRow[] rows { get; }

        public ExcelTableRows(Scenario scenario, Table xclTable)
        {
            // create an array with the rows from the sourcefile
            CellRange xclRange = xclTable.DataRange;
            int lastcol = xclRange.LastColumnIndex;

            IEnumerable<IGrouping<ExcelRow, ExcelCell>> groupedRows = xclRange.GroupBy(groupby => groupby.Row);

            // Rows
            List<TableRow> liRows = new List<TableRow> { };
            foreach (IGrouping<ExcelRow, ExcelCell> row in groupedRows)
            {
                // Cells, concatted into a single TableCell[]
                List<TableCell> tablecell = new List<TableCell> { };
                foreach (ExcelCell cell in row.Key.Cells)
                {
                    if (cell.Column.Index > lastcol)
                        break;

                    TableCell tempcell = new TableCell(scenario.Location, cell.Value.ToString());
                    tablecell.Add(tempcell);
                }
                TableRow currentrow = new TableRow(scenario.Location, tablecell.ToArray<TableCell>());
                liRows.Add(currentrow);
            }
            rows = liRows.ToArray();
        }
    }
}