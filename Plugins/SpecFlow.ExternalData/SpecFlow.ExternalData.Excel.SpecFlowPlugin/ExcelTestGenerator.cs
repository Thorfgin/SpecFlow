using System.IO;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Parser;

namespace SpecFlow.ExternalData.Excel.SpecFlowPlugin
{
    class ExcelTestGenerator : TestGenerator
    {
        private readonly IExcelFeaturePatcher _excelFeaturePatcher;

        public ExcelTestGenerator(SpecFlowConfiguration specFlowConfiguration, ProjectSettings projectSettings, 
            ITestHeaderWriter testHeaderWriter, ITestUpToDateChecker testUpToDateChecker, 
            IFeatureGeneratorRegistry featureGeneratorRegistry, CodeDomHelper codeDomHelper, 
            IGherkinParserFactory gherkinParserFactory, IExcelFeaturePatcher externalDataFeaturePatcher) 
            : base(specFlowConfiguration, projectSettings, testHeaderWriter, testUpToDateChecker, featureGeneratorRegistry, codeDomHelper, gherkinParserFactory)
        {
            _excelFeaturePatcher = externalDataFeaturePatcher;
        }
        
        protected override SpecFlowDocument ParseContent(IGherkinParser parser, TextReader contentReader,
            string documentLocation)
        {
            var document = base.ParseContent(parser, contentReader, documentLocation);
            return _excelFeaturePatcher.PatchDocument(document);
        }
    }

}
