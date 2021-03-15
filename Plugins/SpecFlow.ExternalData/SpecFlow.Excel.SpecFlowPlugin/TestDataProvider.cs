using GemBox.Spreadsheet;
using GemBox.Spreadsheet.Tables;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow.Generator.Interfaces;
using Utf8Json;

namespace SpecFlow.Excel.SpecFlowPlugin
{
    public interface ITestDataProvider
    {
        dynamic TestData { get; }
        dynamic TestSettings { get; }
    }

    public class TestDataProvider : ITestDataProvider
    {
        public dynamic TestData { get; set; }
        public dynamic TestSettings { get; set; }

        public TestDataProvider(ProjectSettings projectSettings)
        {
            TestSettings = projectSettings;
        }

        public dynamic TestDataProviderXLSX(string file, string sheet, string folder = "DataSet\\Excel\\")
        {
            string[] extensions = new string[] { "xlsx", "xls" };
            var ext = file.ToLower().Split('.')[1];

            if (extensions.Contains(ext) == false)
                throw new InvalidDataException($"The file {file} was not accepted as valid 'excel' type file. Use xls or xlsx");

            try
            {
                var path = string.IsNullOrEmpty(TestSettings.ProjectFolder)
                                                ? Directory.GetCurrentDirectory()
                                                : TestSettings.ProjectFolder;

                using (var stream = File.OpenRead(Path.Combine(path, folder, file)))
                {
                    ExcelFile workbook = ExcelFile.Load(stream);
                    ExcelWorksheet worksheet = string.IsNullOrEmpty(sheet)
                                                    ? workbook.Worksheets.First()
                                                    : workbook.Worksheets[sheet];
                    if (worksheet == null)
                        throw new InvalidDataException($"The sheet {sheet} was not found.");

                    Table table = worksheet.Tables.FirstOrDefault();
                    if (table == null || table.Rows.Count == 0)
                        throw new InvalidDataException($"The data on sheet {sheet} was not found.");

                    return table;
                }
            }
            catch { throw new FileNotFoundException($"The file {file} was not found."); }
        }

        public dynamic TestDataProviderJSON(string file, string folder = "DataSet\\JSON\\")
        {
            if (file.ToLower().EndsWith("json") == false)
                throw new InvalidDataException($"The file {file} was not accepted as 'json' type file.");

            try
            {
                var path = string.IsNullOrEmpty(TestSettings.ProjectFolder)
                                ? Directory.GetCurrentDirectory()
                                : TestSettings.ProjectFolder;

                using (var stream = File.OpenRead(Path.Combine(path, folder, file)))
                {
                    return JsonSerializer.Deserialize<dynamic>(stream);
                }
            }
            catch { throw new FileNotFoundException($"The file {file} was not found."); }
        }



    }
}
