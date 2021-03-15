using System;
using System.Collections.Generic;
using System.Linq;
using GemBox.Spreadsheet;
using GemBox.Spreadsheet.Tables;
using Gherkin.Ast;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Parser;

namespace SpecFlow.Excel.SpecFlowPlugin
{
    public interface IExcelFeaturePatcher
    {
        SpecFlowDocument PatchDocument(SpecFlowDocument feature);
    }

    /// <summary>
    /// Collect the data based on the TagInformation
    /// </summary>
    public class ExcelFeaturePatcher : IExcelFeaturePatcher
    {
        public const string PROPERTY_TAG = "source";
        public const string FILETYPE_JSON = "json";
        public const string FILETYPE_XLSX = "xlsx";
        private readonly ITagFilterMatcher _tagFilterMatcher;
        private readonly ITestDataProvider _testDataProvider;

        public ExcelFeaturePatcher(SpecFlowProjectConfiguration configuration, ITagFilterMatcher tagFilterMatcher, ITestDataProvider testDataProvider)
        {
            _tagFilterMatcher = tagFilterMatcher;
            _testDataProvider = testDataProvider;
        }

        public SpecFlowDocument PatchDocument(SpecFlowDocument originalSpecFlowDocument)
        {
            var feature = originalSpecFlowDocument.SpecFlowFeature;
            var scenarioDefinitions = feature.Children.Where(c => c is Background).ToList();

            foreach (var scenario in feature.ScenarioDefinitions.OfType<Scenario>())
            {

                if (!_tagFilterMatcher.GetTagValue(PROPERTY_TAG, scenario.Tags.Select(t => t.Name.Substring(1)),
                    out var tagString))
                {
                    scenarioDefinitions.Add(scenario);
                    continue;
                }

                var newScenarioOutline = PatchScenario(tagString, scenario);

                scenarioDefinitions.Add(newScenarioOutline);
            }

            var newDocument = CreateSpecFlowDocument(originalSpecFlowDocument, feature, scenarioDefinitions);
            return newDocument;
        }

        /// <summary>
        /// Split TagString and Process into new Scenario
        /// </summary>
        /// <param name="tagString"></param>
        /// <param name="scenario"></param>
        /// <returns></returns>
        private ScenarioOutline PatchScenario(string tagString, Scenario scenario)
        {
            // TagStringExample JSON: testdata.json|email=E-mail_addresses.Valid
            // TagStringExample XLSX: testdata.xlsx|Valid

            // Split whole tag into FileType and File/Tab
            var propertyTokens = tagString.Split('|');
            var propertyFile = propertyTokens[0];
            var propertyType = propertyFile.Split('.');
            var propertyValueString = propertyTokens[1];

            // Split by Source into JSON or Excel 
            switch (propertyType[1].ToLower())
            {
                case FILETYPE_JSON:
                    var tokens = propertyValueString.Split('=');
                    var propertyName = tokens[0];
                    var propertyValues = tokens[1];
                    var propertyValueTokensJson = propertyValues.Split('.');
                    return getJsonScenarioOutline(propertyValueTokensJson, propertyName, propertyFile, scenario);
                case FILETYPE_XLSX:
                    return getXlsxScenarioOutline(propertyValueString, propertyFile, scenario);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get the ScenarionOutline for JSON filetype
        /// </summary>
        /// <param name="propertyValueTokens"></param>
        /// <param name="propertyName"></param>
        /// <param name="scenario"></param>
        /// <returns></returns>
        public ScenarioOutline getJsonScenarioOutline(string[] propertyValueTokens, string propertyName, string propertyFile, Scenario scenario)
        {
            var testSettings = _testDataProvider.TestSettings;
            var testValues = new TestDataProvider(testSettings).TestDataProviderJSON(propertyFile);

            foreach (var propertyValueToken in propertyValueTokens)
            {
                var dict = testValues as Dictionary<string, object>;
                if (dict == null)
                    throw new InvalidOperationException($"Cannot resolve properties from {testValues}");

                if (!dict.TryGetValue(propertyValueToken, out testValues) &&
                    !dict.TryGetValue(propertyValueToken.Replace("_", " "), out testValues))
                    throw new InvalidOperationException($"Cannot resolve property {propertyValueToken}");
            }

            var examples = new List<Examples>(scenario.Examples);

            var header = new TableRow(scenario.Location, new TableCell[]
            {
                new TableCell(scenario.Location, "Variant"),
                new TableCell(scenario.Location, propertyName)
            });

            var testValuesDict = testValues as Dictionary<string, object>;

            var rows = testValuesDict
                .Select(p => new TableRow(scenario.Location, new[]
                {
                    new TableCell(scenario.Location, p.Key),
                    new TableCell(scenario.Location, p.Value.ToString()),
                })).ToArray();

            var newExample = new Examples(new Tag[0], scenario.Location, "Examples", string.Empty, string.Empty, header, rows);
            examples.Add(newExample);

            var newScenarioOutline = new ScenarioOutline(scenario.Tags.ToArray(),
                scenario.Location,
                scenario.Keyword,
                scenario.Name,
                scenario.Description,
                scenario.Steps.ToArray(),
                examples.ToArray());
            return newScenarioOutline;
        }

        public ScenarioOutline getXlsxScenarioOutline(string propertySheet, string propertyFile, Scenario scenario)
        {
            var testSettings = _testDataProvider.TestSettings;
            Table xclTable = new TestDataProvider(testSettings).TestDataProviderXLSX(propertyFile, propertySheet);

            // create a headers tablerow
            TableRow xclHeaders = new ExcelTableHeaders(scenario, xclTable).header;

            // create new examples list
            var examples = new List<Examples>(scenario.Examples);

            // create an array with the rows from the sourcefile
            TableRow[] xclRows = new ExcelTableRows(scenario, xclTable).rows; 

            // create a new Example and Add it
            var newExample = new Examples(new Tag[0], scenario.Location, "Examples", string.Empty, string.Empty, xclHeaders, xclRows);
            examples.RemoveAt(0);
            examples.Add(newExample);

            // Add a New Scenario Outline to the test
            var newScenarioOutline = new ScenarioOutline(scenario.Tags.ToArray(),
                scenario.Location,
                scenario.Keyword,
                scenario.Name,
                scenario.Description,
                scenario.Steps.ToArray(),
                examples.ToArray());
            return newScenarioOutline;
        }

        private SpecFlowDocument CreateSpecFlowDocument(SpecFlowDocument originalSpecFlowDocument, SpecFlowFeature originalFeature, List<IHasLocation> scenarioDefinitions)
        {
            var newFeature = new SpecFlowFeature(originalFeature.Tags.ToArray(),
                                                 originalFeature.Location,
                                                 originalFeature.Language,
                                                 originalFeature.Keyword,
                                                 originalFeature.Name,
                                                 originalFeature.Description,
                                                 scenarioDefinitions.ToArray());

            var newDocument = new SpecFlowDocument(newFeature, originalSpecFlowDocument.Comments.ToArray(), originalSpecFlowDocument.DocumentLocation);
            return newDocument;
        }
    }
}