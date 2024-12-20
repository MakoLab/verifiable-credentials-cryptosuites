using DI_Sd_Primitives.Interfaces;

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
                var internalId = entry.Value;
                if (entry.Value.StartsWith("_:"))
                {
                    internalId = internalId[2..];
                }
                if (_labelMap.TryGetValue(internalId, out string? value))
                {
                    bnodeIdMap.Add(entry.Key, value);
                }
            }
            return bnodeIdMap;
        }
    }
}
