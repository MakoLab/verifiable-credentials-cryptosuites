using DI_Sd_Primitives.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI_Sd_Primitives
{
    public class CustomLabelMapFactoryFunction(IDictionary<string, string> labelMap) : ILabelMapFactoryFunction
    {
        private readonly IDictionary<string, string> _labelMap = labelMap;

        public IDictionary<string, string> CreateLabelMap(IDictionary<string, string> canonicalIdMap)
        {
            var bnodeIdMap = new Dictionary<string, string>();
            foreach (var entry in canonicalIdMap)
            {
                if (_labelMap.TryGetValue(entry.Value, out string? value))
                {
                    bnodeIdMap.Add(entry.Key, value);
                }
            }
            return bnodeIdMap;
        }
    }
}
