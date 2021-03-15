using System;
using Xunit;
using GemBox.Spreadsheet;
using GemBox.Spreadsheet.Tables;
using SpecFlow.Excel.SpecFlowPlugin;
using TechTalk.SpecFlow.Generator.Interfaces;
using System.Linq;
using FluentAssertions;

namespace ExcelTests
{
    public class ReadingXclSheet
    {
        private readonly ProjectSettings projectSettings = new ProjectSettings();
        private readonly string strFile = "testdata.xlsx";
        private readonly string strSheet = "Valid";

        [Fact]
        public void ReadTableInformation()
        {
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            ExcelWorksheet xclSheet = new TestDataProvider(projectSettings).TestDataProviderXLSX(strFile, strSheet);
            Table xclTable = xclSheet.Tables.FirstOrDefault();
            xclTable.Rows.Should().NotBeNull();
        }
    }
}

