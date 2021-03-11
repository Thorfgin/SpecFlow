using SpecFlow.ExternalData.Excel.SpecFlowPlugin;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.UnitTestProvider;

[assembly: GeneratorPlugin(typeof(ExcelPlugin))]

namespace SpecFlow.ExternalData.Excel.SpecFlowPlugin
{
    public class ExcelPlugin : IGeneratorPlugin
    {
        public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters,
            UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            generatorPluginEvents.RegisterDependencies += (sender, args) =>
            {
                args.ObjectContainer.RegisterTypeAs<ExcelTestGenerator, ITestGenerator>();
                args.ObjectContainer.RegisterTypeAs<ExcelFeaturePatcher, IExcelFeaturePatcher>();
                args.ObjectContainer.RegisterTypeAs<TestDataProvider, ITestDataProvider>();
            };
        }
    }
}

