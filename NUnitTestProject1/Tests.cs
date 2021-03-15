using GemBox.Spreadsheet;
using GemBox.Spreadsheet.Tables;
using Gherkin.Ast;
using NUnit.Framework;
using SpecFlow.Excel.SpecFlowPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace Tests
{
    public class Tests
    {
        public ProjectSettings projectSettings;
        public string root;
        public string folder;
        string file;

        [SetUp]
        public void Setup()
        {
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            projectSettings = new ProjectSettings();
            root = Directory.GetCurrentDirectory();
            folder = "DataSet\\Excel\\";
            file = "testdata.xlsx";
        }

        [Test]
        public void CreateADataTable()
        {
            string sheet = "Valid";
            string path = Path.Combine(root, folder, file);

            Assert.IsTrue(File.Exists(path), $"The file was not found at path: {path}");

            using (var stream = File.OpenRead(path))
            {
                ExcelFile workbook = ExcelFile.Load(stream);
                ExcelWorksheet worksheet = string.IsNullOrEmpty(sheet)
                                                ? workbook.Worksheets.First()
                                                : workbook.Worksheets[sheet];

                if (worksheet == null)
                    throw new InvalidDataException($"The sheet {sheet} was not found.");

                Table xclTable = worksheet.Tables.FirstOrDefault();
                Assert.IsTrue(xclTable != null && xclTable.Rows.Count != 0);

                Table xclTableFromFunction = new TestDataProvider(projectSettings).TestDataProviderXLSX(file, sheet, folder);
            }
        }

        [Test]
        public void GetHeaders()
        {
            string sheet = "Valid";
            Table xclTable = new TestDataProvider(projectSettings).TestDataProviderXLSX(file, sheet, folder);

            Location location = new Location();
            List<TableCell> tablecell = new List<TableCell> { };

            foreach (TableColumn col in xclTable.Columns)
            {
                tablecell.Add(new TableCell(location, col.Name));
            }

            TableRow header = new TableRow(location, tablecell.ToArray<TableCell>());
            Assert.IsNotNull(header);
            Assert.NotZero(header.Cells.Count());
        }

        [Test]
        public void GetRows()
        {
            string sheet = "Valid";
            Table xclTable = new TestDataProvider(projectSettings).TestDataProviderXLSX(file, sheet, folder);

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

                    TableCell tempcell = new TableCell(new Location(), cell.Value.ToString());
                    tablecell.Add(tempcell);
                }
                TableRow currentrow = new TableRow(new Location(), tablecell.ToArray<TableCell>());
                liRows.Add(currentrow);
            }
            TableRow[] rows = liRows.ToArray();
            Assert.IsNotNull(rows);
            Assert.NotZero(rows.Count());
        }

        [Test]
        public void GetVars()
        {
            var testValues = new TestDataProvider(projectSettings).TestDataProviderJSON("testdata.json");

            // Split whole tag into FileType and File/Tab
            var tagString = "testdata.json|email=E-mail_addresses.Valid";
            var propertyTokens = tagString.Split('|');
            var propertyFile = propertyTokens[0];
            var propertyType = propertyFile.Split('.');
            var propertyValueString = propertyTokens[1];

            var tokens = propertyValueString.Split('=');
            var propertyName = tokens[0];
            var propertyValues = tokens[1];
            var propertyValueTokensJson = propertyValues.Split('.');

            foreach (var propertyValueToken in propertyValueTokensJson)
            {
                var dict = testValues as Dictionary<string, object>;
                
                if (!dict.TryGetValue(propertyValueToken, out testValues) &&
                    !dict.TryGetValue(propertyValueToken.Replace("_", " "), out testValues))
                    throw new InvalidOperationException($"Cannot resolve property {propertyValueToken}");
            }

            var testValuesDict = testValues as Dictionary<string, object>;

            var rows = testValuesDict
                .Select(p => new TableRow(new Location(), new[]
                {
                    new TableCell(new Location(), p.Key),
                    new TableCell(new Location(), p.Value.ToString()),
                })).ToArray();
        }
    }
}