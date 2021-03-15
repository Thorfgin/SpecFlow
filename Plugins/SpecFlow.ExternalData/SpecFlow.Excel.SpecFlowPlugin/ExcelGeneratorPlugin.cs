using GemBox.Spreadsheet;
using SpecFlow.Excel.SpecFlowPlugin;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.UnitTestProvider;

[assembly:GeneratorPlugin(typeof(ExcelGeneratorPlugin))]

namespace SpecFlow.Excel.SpecFlowPlugin
{
    public class ExcelGeneratorPlugin : IGeneratorPlugin
    {
        public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters,
            UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

            generatorPluginEvents.RegisterDependencies += (sender, args) =>
            {
                args.ObjectContainer.RegisterTypeAs<ExcelTestGenerator, ITestGenerator>();
                args.ObjectContainer.RegisterTypeAs<ExcelFeaturePatcher, IExcelFeaturePatcher>();
                args.ObjectContainer.RegisterTypeAs<TestDataProvider, ITestDataProvider>();
            };
        }
    }
}
