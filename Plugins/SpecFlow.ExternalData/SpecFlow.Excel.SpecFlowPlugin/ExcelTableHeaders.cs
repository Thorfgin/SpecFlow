using GemBox.Spreadsheet.Tables;
using Gherkin.Ast;
using System.Collections.Generic;
using System.Linq;

namespace SpecFlow.Excel.SpecFlowPlugin
{
    class ExcelTableHeaders
    {
        public TableRow header { get; }

        public ExcelTableHeaders(Scenario scenario, Table xclTable)
        {
            // create a TableRow with Headers
            List<TableCell> tablecell = new List<TableCell> { };

            foreach (TableColumn col in xclTable.Columns)
            {
                if (string.IsNullOrEmpty(col.Name))
                    break;

                tablecell.Add(new TableCell(scenario.Location, col.Name));
            }

            header = new TableRow(scenario.Location, tablecell.ToArray<TableCell>());
        }
    }
}
