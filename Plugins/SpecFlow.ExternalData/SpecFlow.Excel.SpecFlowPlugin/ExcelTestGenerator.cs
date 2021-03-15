using System.IO;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Parser;

namespace SpecFlow.Excel.SpecFlowPlugin
{
    public class ExcelTestGenerator : TestGenerator
    {
        private readonly IExcelFeaturePatcher _excelFeaturePatcher;
        public ExcelTestGenerator(SpecFlowConfiguration specFlowConfiguration, ProjectSettings projectSettings, ITestHeaderWriter testHeaderWriter, ITestUpToDateChecker testUpToDateChecker, IFeatureGeneratorRegistry featureGeneratorRegistry, CodeDomHelper codeDomHelper, IGherkinParserFactory gherkinParserFactory, IExcelFeaturePatcher excelFeaturePatcher) : base(specFlowConfiguration, projectSettings, testHeaderWriter, testUpToDateChecker, featureGeneratorRegistry, codeDomHelper, gherkinParserFactory)
        {
            _excelFeaturePatcher = excelFeaturePatcher;
        }

        protected override SpecFlowDocument ParseContent(IGherkinParser parser, TextReader contentReader,
            SpecFlowDocumentLocation documentLocation)
        {
            var document = base.ParseContent(parser, contentReader, documentLocation);
            return _excelFeaturePatcher.PatchDocument(document);
        }
    }
}
