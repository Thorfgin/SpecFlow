using System.Collections.Generic;
using System.Linq;

namespace SpecFlow.Excel.SpecFlowPlugin
{
    class ExamplesHeader
    {
        public ExamplesHeader(IEnumerable<string> values)
        {
            Values = values.ToList();
            Count = Values.Count();
        }

        public int Count { get; }

        public IEnumerable<string> Values { get; }
    }


}
